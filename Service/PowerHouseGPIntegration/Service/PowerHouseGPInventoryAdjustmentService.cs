using BSP.PowerHouse.DynamicsGP.Integration.Configuration;
using BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects;
//using BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects;
using BSP.PowerHouse.DynamicsGP.Integration.Model;
using BSP.PowerHouse.DynamicsGP.Integration.PowerHouseWS;
using BSP.PowerHouse.DynamicsGP.Integration.Tools;
using Microsoft.Dynamics.GP.eConnect;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BSP.PowerHouse.DynamicsGP.Integration.Service
{
    public class PowerHouseGPInventoryAdjustmentService : IPowerHouseGPService
    {
        private readonly PowerhouseWsSetting _powerhouseWsSetting;
        public PowerHouseGPInventoryAdjustmentService(PowerhouseWsSetting powerhouseWsSetting)
        {
            this._powerhouseWsSetting = powerhouseWsSetting;
        }
        public void Process()
        {
            PHWebServicesPortTypeClient serviceProxy = new PHWebServicesPortTypeClient(AppSettings.PowerhouseEndpoint);
            string sessionId = null;
            try
            {
                //connect
                sessionId = serviceProxy.reLogIn(null, _powerhouseWsSetting.USERID, _powerhouseWsSetting.BSPPowerhousePassword);
                if (!string.IsNullOrWhiteSpace(sessionId))
                {
                    var inventoryAdjustments = serviceProxy.getInventoryAdjustments(sessionId);

                    //flag not posted transations as done
                    if (AppSettings.TracingEnabled)
                    {
                        Utility.LogResult(inventoryAdjustments, "inventory_adjustments");
                    }
                    //check if any thing returned, checking null for first element because the service returns an array with one null element if no data
                    if (inventoryAdjustments != null && inventoryAdjustments.Length > 0 && inventoryAdjustments[0] != null)
                    {
                        // Testing
                        //var test = inventoryAdjustments.Where(t => t.ifTranCode.Equals("POST-INC", StringComparison.OrdinalIgnoreCase)).ToList();
                        string transferBatch;

                        //process receivings
                        var receivingTrxs = inventoryAdjustments.Where(t => t.ifTranCode.Equals("POST-INC", StringComparison.OrdinalIgnoreCase)).ToList();                        
                        if (receivingTrxs.Count > 0)
                        {
                            var poReceivingTrxs = inventoryAdjustments.Where(t => t.receiptType != null && t.receiptType.Equals(_powerhouseWsSetting.BSPPHPOReceivingType)).ToList();
                            if (poReceivingTrxs.Count > 0)
                            {
                                ProcessReceivingTransactions(serviceProxy, sessionId, poReceivingTrxs);
                            }

                            var containerReceivingTrxs = inventoryAdjustments.Where(t => t.receiptType != null && t.receiptType.Equals(_powerhouseWsSetting.BSPPHContReceivingType)).ToList();
                            if (containerReceivingTrxs.Count > 0)
                            {
                                transferBatch = CreateBatch(new InventoryTransferEntryBatch(_powerhouseWsSetting));

                                foreach (var containerReceivingTrx in containerReceivingTrxs)
                                {
                                    ProcessInventoryTransaction(serviceProxy, sessionId, new IVInventoryTransfer(transferBatch, containerReceivingTrx, _powerhouseWsSetting), containerReceivingTrx, GPIvTrxType.Transfer);
                                }
                            }
                        }

                        //process inventory adjustments
                        var inventoryTrxs = inventoryAdjustments.Where(t => !t.ifTranCode.Equals("POST-INC", StringComparison.OrdinalIgnoreCase))
                            .OrderBy(t => t.activityDate)
                            .ThenBy(t => t.activityTime)
                            .ThenBy(t => t.ifSeqNum)
                            .ToList();

                        string trxBatch = string.Empty;
                        transferBatch = string.Empty;

                        if (inventoryTrxs.Count > 0)
                        {
                            trxBatch = CreateBatch(new InventoryTrxEntryBatch(_powerhouseWsSetting));
                            //transferBatch = CreateBatch(new InventoryTransferEntryBatch(_powerhouseWsSetting));
                        }

                        foreach (var ivTran in inventoryTrxs)
                        {
                            switch (ivTran.ifTranCode)
                            {
                                case "DEC":
                                case "CY-TOT-INC":
                                case "INC":
                                case "BH-CONT":
                                case "CY-TOT-DEC":
                                case "CHG-DEC":
                                case "CHG-INC":                                                                                                
                                    //inventory adjustment
                                    ProcessInventoryTransaction(serviceProxy, sessionId, new IVInventoryTransaction(trxBatch, ivTran, _powerhouseWsSetting), ivTran, GPIvTrxType.Adjustment);
                                    break;
                                default:
                                    break;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                EmailHelper.SendEmail(AppSettings.EmailSubject + " PH Inventory Adjustment General Exception Failure ", ex.Message);

                EventLogUtility.LogException(ex);
            }
            finally
            {
                if (serviceProxy != null)
                {
                    try
                    {
                        serviceProxy.logOut(sessionId);
                    }
                    catch { }
                }
            }
        }

        private void ProcessInventoryTransaction(PHWebServicesPortTypeClient serviceProxy, string sessionId, IEConnectIVObject gpIVTransaction, InventoryAdjustment invTrx, GPIvTrxType trxtype)
        {
            try
            {
                using (eConnectMethods econnect = new eConnectMethods())
                {
                    string sDocumentXml = gpIVTransaction.GetXmlSerializedObject();
                    if (AppSettings.TracingEnabled)
                    {
                        EventLogUtility.LogInformationMessage(sDocumentXml);
                    }
                    if(gpIVTransaction.ItemSites.Count > 0)
                    {
                        var ivItemSite = new InventoryItemSite(gpIVTransaction.ItemSites);
                        string itemSitesXML = ivItemSite.GetXmlSerializedObject();
                        econnect.CreateEntity(Utility.GetEconnectConnectionString(AppSettings.GPConnectionString), itemSitesXML);
                    }
                    var result = econnect.CreateTransactionEntity(Utility.GetEconnectConnectionString(AppSettings.GPConnectionString), sDocumentXml);

                    //update power house flags
                    if (AppSettings.SendShipResponse)
                    {
                        var invAdjResponses = new InventoryAdjustmentResponse[]
                        {
                                new InventoryAdjustmentResponse
                                {
                                    ifSeqNum = invTrx.ifSeqNum,
                                    ifSeqNumSpecified = true,
                                    warehouseIdFrom = invTrx.warehouseIdFrom,
                                    ifStatus = "X",
                                }
                        };
                        serviceProxy.sendInvAdjResponses(sessionId, invAdjResponses);
                    }

                    if (AppSettings.AutoPostInventoryTrx && trxtype == GPIvTrxType.Adjustment)
                    {
                        //call post
                        if (!string.IsNullOrWhiteSpace(_powerhouseWsSetting.BSPGPWSURL))
                        {
                            var client = new RestClient(_powerhouseWsSetting.BSPGPWSURL)
                            {
                                Timeout = 600000
                            };

                            var resource = $"Tenants(DefaultTenant)/Companies(" + AppSettings.GPCompanyName + ")/BusinessSolutionPartners/Inventory/Transactions({Type};{Number})";
                            var request = new RestRequest(resource, Method.POST);

                            //Temporary only for testing
                            //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                //System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };

                            if (!AppSettings.GPSBAUseDefaultCredentials)
                            {
                                client.Authenticator = new NtlmAuthenticator(
                                    new NetworkCredential(
                                        AppSettings.GPSBAUserId,
                                        AppSettings.GPSBAPassword,
                                        AppSettings.GPSBADomain));
                            }
                            else
                            {
                                request.UseDefaultCredentials = true;
                            }

                            request.AddHeader("GP-Custom-Action", "POST");
                            request.AddUrlSegment("Type", (int)trxtype);
                            request.AddUrlSegment("Number", invTrx.ifSeqNum.Value.ToString());

                            var response = client.Execute(request);
                            if (!response.IsSuccessful)
                            {
                                //log error
                                var errorMsg = $"GP Service Error:\r\n\r\nResource: {request.Resource} \r\n\r\nParameters: \r\n\r\n{string.Join("\r\n", request.Parameters)} \r\n\r\nResponse Content: \r\n\r\n{response.Content}";
                                EventLogUtility.LogWarningMessage(errorMsg);
                            }
                        }
                    }                    
                }
            }
            catch (eConnectException exc)
            {
                EmailHelper.SendEmail(AppSettings.EmailSubject + " PH Inventory Transaction Econnect Failure ", exc.Message);

                //update powerhouse if tried before
                if (invTrx.ifStatus.HasValue && invTrx.ifStatus.Value > 1 && AppSettings.SendShipResponse)
                {
                    var invAdjResponses = new InventoryAdjustmentResponse[]
                    {
                        new InventoryAdjustmentResponse
                        {
                            ifSeqNum = invTrx.ifSeqNum,
                            ifSeqNumSpecified = true,
                            warehouseIdFrom = invTrx.warehouseIdFrom,
                            ifStatus = "E",
                            ifErrorMsg = exc.Message
                        }
                    };
                    serviceProxy.sendInvAdjResponses(sessionId, invAdjResponses);
                }
                EventLogUtility.LogException(exc);
            }
            catch (Exception exc)
            {
                EmailHelper.SendEmail(AppSettings.EmailSubject + " PH Inventory Transaction General Failure ", exc.Message);

                EventLogUtility.LogException(exc);
            }
        }
        private void ProcessReceivingTransactions(PHWebServicesPortTypeClient serviceProxy, string sessionId, List<InventoryAdjustment> receivingTrxs)
        {
            //create batch
            string batchNumber = CreateBatch(new ReceivingsTrxEntryBatch(_powerhouseWsSetting));

            var result = from receivingTrx in receivingTrxs
                         group receivingTrx by receivingTrx.poId;

            var receipts = new List<ReceiptTransaction>();
            foreach (var group in result)
            {
                var receipt = new ReceiptTransaction
                {
                    PoNumber = group.Key,
                    BatchNumber = batchNumber
                };
                foreach (var value in group)
                {
                    receipt.InventoryAdjustments.Add(value);
                }
                receipts.Add(receipt);
            }

            foreach (var receipt in receipts)
            {
                var documentRollBack = new DocumentRollback();
                try
                {
                    // Instantiate a GetNextDocNumbers object
                    using (var gpGetNextDocNumbers = new GetNextDocNumbers())
                    {
                        // generate a receipt number
                        var receiptNumber = gpGetNextDocNumbers.GetNextPOPReceiptNumber(IncrementDecrement.Increment, Utility.GetEconnectConnectionString(AppSettings.GPConnectionString));
                        if (!string.IsNullOrWhiteSpace(receiptNumber))
                        {
                            receipt.ReceiptNumber = receiptNumber.Trim();
                        }
                    }
                    var gpReceiptTransaction = new PopRcptTransaction(receipt);
                    using (eConnectMethods econnect = new eConnectMethods())
                    {
                        string sDocumentXml = gpReceiptTransaction.GetXmlSerializedObject();
                        if (AppSettings.SendShipResponse)
                        {
                            EventLogUtility.LogInformationMessage(sDocumentXml);
                        }
                        var receiptTran = econnect.CreateTransactionEntity(Utility.GetEconnectConnectionString(AppSettings.GPConnectionString), sDocumentXml);
                        if (!string.IsNullOrWhiteSpace(receiptTran))
                        {
                            //update power house flags
                            if (AppSettings.SendShipResponse)
                            {
                                var invAdjResponses = new List<InventoryAdjustmentResponse>();
                                foreach (var invAdj in receipt.InventoryAdjustments)
                                {
                                    invAdjResponses.Add(new InventoryAdjustmentResponse
                                    {
                                        ifSeqNum = invAdj.ifSeqNum,
                                        ifSeqNumSpecified = true,
                                        warehouseIdFrom = invAdj.warehouseIdFrom,
                                        ifStatus = "X",
                                    });
                                }
                                serviceProxy.sendInvAdjResponses(sessionId, invAdjResponses.ToArray());
                            }
                        }                        
                    }
                }
                catch (eConnectException exc)
                {
                    EmailHelper.SendEmail(AppSettings.EmailSubject + " PH Receiving Download Econnect Failure " + receipt.ReceiptNumber + "." , exc.Message);

                    if (!string.IsNullOrWhiteSpace(receipt.ReceiptNumber))
                    {
                        documentRollBack.Add(TransactionType.POPReceipt, receipt.ReceiptNumber);
                    }
                    //update powerhouse if tried before
                    if (receipt.InventoryAdjustments.Exists(a => a.ifStatus.HasValue && a.ifStatus.Value > 1) && AppSettings.SendShipResponse)
                    {
                        //update powerhouse
                        var invAdjResponses = new List<InventoryAdjustmentResponse>();
                        foreach (var invAdj in receipt.InventoryAdjustments)
                        {
                            invAdjResponses.Add(new InventoryAdjustmentResponse
                            {
                                ifSeqNum = invAdj.ifSeqNum,
                                ifSeqNumSpecified = true,
                                warehouseIdFrom = invAdj.warehouseIdFrom,
                                ifStatus = "E",
                                ifErrorMsg = Utility.GetErrorDescription(exc.Message)
                            });
                        }
                        serviceProxy.sendInvAdjResponses(sessionId, invAdjResponses.ToArray());
                    }
                    EventLogUtility.LogException(exc);
                }
                catch (Exception exc)
                {
                    EmailHelper.SendEmail(AppSettings.EmailSubject + " PH Receiving Download General Failure " + receipt.ReceiptNumber + ".", exc.Message);

                    if (!string.IsNullOrWhiteSpace(receipt.ReceiptNumber))
                    {
                        documentRollBack.Add(TransactionType.POPReceipt, receipt.ReceiptNumber);
                    }
                    EventLogUtility.LogException(exc);
                }
                finally
                {
                    if (documentRollBack.CollectionContainsDocuments())
                    {
                        using (var gpGetNextDocNumbers = new GetNextDocNumbers())
                        {
                            gpGetNextDocNumbers.SortAndRollBackDocumentList(documentRollBack.GetRollBackDocumentList(),
                                Utility.GetEconnectConnectionString(AppSettings.GPConnectionString));
                        }
                    }
                }
            }
        }

        private string CreateBatch(SMTransactionBatch gpBatch)
        {
            string batchNumber = string.Empty;

            string sDocumentXml = gpBatch.GetXmlSerializedObject();
            if (!string.IsNullOrWhiteSpace(sDocumentXml))
            {
                using (eConnectMethods econnect = new eConnectMethods())
                {
                    if (econnect.CreateEntity(Utility.GetEconnectConnectionString(AppSettings.GPConnectionString), sDocumentXml))
                    {
                        batchNumber = gpBatch.BatchNumber;
                    }

                }
            }
            return batchNumber;
        }
    }
}
