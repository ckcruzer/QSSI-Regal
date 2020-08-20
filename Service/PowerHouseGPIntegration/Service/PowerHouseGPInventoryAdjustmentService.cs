﻿using BSP.PowerHouse.DynamicsGP.Integration.Configuration;
using BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects;
//using BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects;
using BSP.PowerHouse.DynamicsGP.Integration.Model;
using BSP.PowerHouse.DynamicsGP.Integration.PowerHouseWS;
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
                        //var inventoryTrxs = inventoryAdjustments.Where(t => !t.ifTranCode.Equals("POST-INC", StringComparison.OrdinalIgnoreCase))
                        //    .OrderBy(t => t.activityDate)
                        //    .ThenBy(t => t.activityTime)
                        //    .ThenBy(t => t.ifSeqNum)
                        //    .ToList();

                        //string trxBatch = string.Empty;
                        //transferBatch = string.Empty;

                        //if (inventoryTrxs.Count > 0)
                        //{
                        //    trxBatch = CreateBatch(new InventoryTrxEntryBatch(_powerhouseWsSetting));
                        //    transferBatch = CreateBatch(new InventoryTransferEntryBatch(_powerhouseWsSetting));
                        //}

                        //foreach (var ivTran in inventoryTrxs)
                        //{
                        //    switch (ivTran.ifTranCode)
                        //    {
                        //        case "BH-CONT":
                        //        case "CHG-DEC":
                        //        case "CY-DEC":
                        //        case "DEC":
                        //        case "DEC-AUTO":
                        //        case "CHG-INC":
                        //        case "CY-INC":
                        //        case "INC":
                        //        case "INC-AUTO":
                        //            //inventory adjustment
                        //            ProcessInventoryTransaction(serviceProxy, sessionId, new IVInventoryTransaction(trxBatch, ivTran, _powerhouseWsSetting), ivTran, GPIvTrxType.Adjustment);
                        //            break;
                        //        case "HOLD-PLACE":
                        //        case "HOLD-REL":                                                                       
                        //        default:
                        //            break;
                        //    }
                        //}

                    }
                }
            }
            catch (Exception ex)
            {
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
                }
            }
            catch (eConnectException exc)
            {
                //update powerhouse if tried before
                if (invTrx.ifStatus.HasValue && invTrx.ifStatus.Value > 1)
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
