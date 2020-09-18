using Microsoft.Dexterity.Applications;
using Microsoft.Dexterity.Bridge;
using System;
using BspDictionary = Microsoft.Dexterity.Applications.BusinessSolutionPartnersDictionary;
using BSP.DynamicsGP.PowerHouse.PowerhouseWS;
using BSP.DynamicsGP.PowerHouse.Models;
using System.Collections.Generic;
using System.Linq;
using BSP.DynamicsGP.PowerHouse.Extensions;

namespace BSP.DynamicsGP.PowerHouse
{
    [SupportedDexPlatforms(DexPlatforms.DesktopClient | DexPlatforms.WebClient | DexPlatforms.Service)]
    public class GPAddIn : IDexterityAddIn
    {
        // Create a reference to the Sales Transaction Entry form
        //public static BspDictionary.SopEntryForm SopEntryForm = BusinessSolutionPartners.Forms.SopEntry;
        //// Create a reference to the Sales Transaction Entry window
        //public static BspDictionary.SopEntryForm.SopEntryWindow SopEntryWindow = SopEntryForm.SopEntry;

        //// Create a reference to the POP Transaction Entry form
        //private static BspDictionary.PopPoEntryForm POEntryForm = BusinessSolutionPartners.Forms.PopPoEntry;
        //// Create a reference to the POP Transaction Entry window
        //private static BspDictionary.PopPoEntryForm.PopPoEntryWindow POEntryWindow = POEntryForm.PopPoEntry;


        //// Create a reference to the IV Item Maintenance form
        //private static BspDictionary.IvItemMaintenanceForm IVItemMaintForm = BusinessSolutionPartners.Forms.IvItemMaintenance;
        //// Create a reference to the IV Item Maintenance window
        //private static BspDictionary.IvItemMaintenanceForm.IvItemMaintenanceWindow IVItemMaintWindow = IVItemMaintForm.IvItemMaintenance;

        //private static BspDictionary.BspPowerhouseExportUtilityForm ExportUtilForm = BusinessSolutionPartners.Forms.BspPowerhouseExportUtility;

        public static BspDictionary.BspIvItemMaintenanceIntegrationForm BspIvItemMaintenanceIntegrationForm = BusinessSolutionPartners.Forms.BspIvItemMaintenanceIntegration;
        public static Microsoft.Dexterity.Applications.DynamicsDictionary.IvItemMaintenanceForm IvItemMaintenanceForm = Dynamics.Forms.IvItemMaintenance;

        public static BspDictionary.BspSopEntryIntegrationForm BspSopEntryIntegrationForm = BusinessSolutionPartners.Forms.BspSopEntryIntegration;
        public static Microsoft.Dexterity.Applications.DynamicsDictionary.SopEntryForm SopEntryForm = Dynamics.Forms.SopEntry;
        public static Microsoft.Dexterity.Applications.DynamicsDictionary.SopEntryForm.SopEntryWindow SopEntryWindow = SopEntryForm.SopEntry;

        public static BspDictionary.BspPopReceivingIntegrationForm BspPopReceivingIntegrationForm = BusinessSolutionPartners.Forms.BspPopReceivingIntegration;
        public static BspDictionary.BspPopReceivingIntegrationMaintenanceForm BspPopReceivingIntegrationMaintenanceForm = BusinessSolutionPartners.Forms.BspPopReceivingIntegrationMaintenance;

        private static Microsoft.Dexterity.Applications.DynamicsDictionary.PopPoEntryForm POEntryForm = Dynamics.Forms.PopPoEntry;
        private static Microsoft.Dexterity.Applications.DynamicsDictionary.PopPoEntryForm.PopPoEntryWindow POEntryWindow = POEntryForm.PopPoEntry;

        private static BspDictionary.BspPopPoEntryIntegrationForm BspPopPoEntryIntegrationForm = BusinessSolutionPartners.Forms.BspPopPoEntryIntegration;

        private string _containerID = string.Empty;
        private string _sopNumber = string.Empty;
        private short _sopType = 0;
        private string _phWarehouseID = string.Empty;
        private string _poNumber = string.Empty;


        private PowerhouseWsSetting _powerhouseWsSettings;

        private string _itemNumber = string.Empty;

        public void Initialize()
        {

            IvItemMaintenanceForm.IvItemMaintenance.SaveButton.ClickAfterOriginal += IVItemMaintWindowSaveButton_ClickAfterOriginal;
            BspIvItemMaintenanceIntegrationForm.Procedures.SendToPowerhouse.InvokeAfterOriginal += InventoryItemSendToPowerhouse_InvokeAfterOriginal;

            BspSopEntryIntegrationForm.Procedures.SendToPowerhouse.InvokeAfterOriginal += SendToPowerhouse_InvokeAfterOriginal;
            BusinessSolutionPartners.Procedures.BspTriggerFunctionDeleteSopEntryProc.InvokeAfterOriginal += BspTriggerFunctionDeleteSopEntryProc_InvokeAfterOriginal;
            BusinessSolutionPartners.Procedures.BspTriggerFunctionVoidSopEntryProc.InvokeAfterOriginal += BspTriggerFunctionVoidSopEntryProc_InvokeAfterOriginal;

            BspPopReceivingIntegrationForm.Procedures.SendToPowerhouse.InvokeAfterOriginal += ReceivingSendToPowerhouse_InvokeAfterOriginal;
            BspPopReceivingIntegrationMaintenanceForm.Procedures.DeleteRecord.InvokeAfterOriginal += ReceivingDeleteRecord_InvokeAfterOriginal;

            BspPopPoEntryIntegrationForm.Procedures.SendToPowerhouse.InvokeAfterOriginal += POSendToPowerhouse_InvokeAfterOriginal;
        }
        
        #region Sales Order Methods
        private void SendToPowerhouse_InvokeAfterOriginal(object sender, BspDictionary.BspSopEntryIntegrationForm.SendToPowerhouseProcedure.InvokeEventArgs e)
        {
            _sopType = e.inParam1;
            _sopNumber = e.inParam2;

            try
            {

                _powerhouseWsSettings = DataAccessHelper.GetPowerhouseWsSettings();
                if (_powerhouseWsSettings == null)
                {
                    throw new Exception("Powerhouse Integration setting is missing!");
                }

                var salesOrder = DataAccessHelper.GetSalesTransactionWork(_sopType, _sopNumber);
                if (salesOrder == null)
                {
                    throw new Exception("Transaction not found!");

                }

                var siteId = DataAccessHelper.GetLocationMapping(salesOrder.SiteID);
                if (string.IsNullOrEmpty(siteId))
                {
                    throw new Exception("Location mapping not found!");
                }
                else
                {
                    salesOrder.SiteID = siteId;
                }

                //Double check again to make sure that the batch belongs to sales order batch in the setup
                if (salesOrder.BatchNumber != _powerhouseWsSettings.SalesBatchID)
                {
                    throw new Exception("Invalid Order Batch");
                }


                var responses = SendSalesOrder(salesOrder);
                if (responses != null)
                {   
                    var errors = new List<string>();
                    var releaseNum = string.Empty;
                    foreach (var response in responses)
                    {
                        if (!string.IsNullOrWhiteSpace(response.ifErrorMsg))
                        {
                            errors.Add(response.ifErrorMsg);
                        }
                        else
                        {
                            e.outParam3 = 0;
                            DataAccessHelper.LogSoTransfer(_sopType, _sopNumber, response.releaseNum, response.ifStatus, response.ifErrorMsg);
                        }

                        releaseNum = response.releaseNum;
                        BspSopEntryIntegrationForm.BspSopEntryIntegration.VersionNumber.Value = releaseNum;
                    }
                    if (errors.Count > 0)
                    {
                        throw new Exception(errors[0]); //Just retrieve the first one
                    }
                }
                else
                {
                    throw new Exception("NULL Response Returned");
                }
            }
            catch (Exception ex)
            {
                DataAccessHelper.LogSoTransfer(_sopType, _sopNumber, string.Empty, "B", ex.Message);

                e.outParam4 = ex.Message;
                e.outParam3 = 1;
            }
        }


        private OrderResponse[] SendSalesOrder(SalesTransaction salesOrder)
        {
            var client = new PHWebServices();
            client.Url = _powerhouseWsSettings.Url;
            string sessionId = null;
            try
            {
                var order = GetOrder(salesOrder);
                //set required flags
                order.warehouseId = salesOrder.SiteID;
                order.ownerId = _powerhouseWsSettings.OwnerId;
                order.releaseNum = "0";
                order.shipByDate = salesOrder.RequestedShipDate;
                order.shipByDateSpecified = true;

                // Added as requested by Margie
                order.group1 = salesOrder.CustomerName;

                // According to Bahaa, always treat orders now as "new" and should always be sent as Y

                //new
                order.ifActionId = Constants.PowerhouseIfAction.RESET;
                order.dateCreated = salesOrder.CreatedDate;
                order.dateCreatedSpecified = true;
                order.createdUsrId = Dynamics.Globals.UserId.Value;
                order.totalValue = order.totalValue.Value.RoundUp(2);

                //connect
                sessionId = client.reLogIn(null, _powerhouseWsSettings.PowerhouseUserId, _powerhouseWsSettings.PowerhousePassword);
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    if (!Constants.SuppressUserErrorNotification)
                    {
                        throw new Exception("Cannot Establish a connection to Powerhouse Webservices.");
                    }
                }
#if DEBUG
                //log results
                //LogResult(orders.ToArray());
#endif
                //we need to round the order totals here before sending

                var responses = client.sendOrdersTran(sessionId, new Order[] { order });
                if (responses != null)
                {
                    if (responses.Count() > 0 && responses[0] != null)
                    {
                        if (responses.Any(r => !string.IsNullOrWhiteSpace(r.ifErrorMsg)))
                        {
                            return responses;
                        }

                        //clear previous and log
                        DataAccessHelper.UpdateSalesOrderRequests(order);
                    }
                    else
                        throw new Exception("Powerhouse integration was unsuccesful.");
                }
                else
                    throw new Exception("Powerhouse integration was unsuccesful.");
                return responses;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (client != null)
                {
                    try
                    {
                        client.logOut(sessionId);
                    }
                    catch
                    {
                        //don't care about this exception, if happened.
                    }
                }
            }
        }

        private OrderLine[] AddOrderLines(Order order, SalesTransaction salesOrder)
        {
            //add lines
            var orderLines = new List<OrderLine>();
            foreach (var line in salesOrder.Items)
            {
                if (line.ShouldBeSentToPowerHouse)
                {
                    var ol = GetOrderLine(line, order.pickCompleteFlag);
                    orderLines.Add(ol);

                    //update order total
                    if (!order.totalValue.HasValue)
                    {
                        //beacuse it will be null
                        order.totalValue = 0;
                    }
                    order.totalValue += (ol.price.HasValue && ol.piecesToPick.HasValue) ? ol.price.Value * ol.piecesToPick.Value : 0;
                    order.totalValueSpecified = true;
                }
            }

            return orderLines.ToArray();
        }

        private OrderLine GetOrderLine(SalesTransactionLine line, string pickCompleteFlag)
        {
            decimal qtyToPick = _powerhouseWsSettings.SOQtyToUse == 1 ? line.Qty - (line.QtyToBackOrder + line.QtyCancelled) : line.QtyAllocated;


            return new OrderLine
            {
                hostLine = line.LineItemSequence.ToString(),
                hostId1 = line.ComponentSequence.ToString(),
                itemId = line.ItemNumber?.SanitizeXMLString(),
                custItemId = line.CustomerItem?.CustomerItemNumber?.SanitizeXMLString(),
                orderLine = line.LineItemSequence + (line.ComponentSequence / 16384),
                orderLineSpecified = true,
                piecesOrdered = Convert.ToDouble((line.Qty - line.QtyCancelled) * line.QtyInBaseUOfM),
                piecesOrderedSpecified = true,
                pickCompleteFlag = pickCompleteFlag,
                //RIC: Added to check which qty to use
                piecesToPick = line.ItemType == 3 && line.ComponentSequence == 0 ? 0 : Convert.ToDouble(qtyToPick * line.QtyInBaseUOfM), // This should be zero if this is the "main" kit item. if inventory item, normal.
                piecesToPickSpecified = true,
                price = Convert.ToDouble(line.UnitPrice / line.QtyInBaseUOfM),
                priceSpecified = true,
                olCust1 = line.UnitPrice.ToString(),
                olCust2 = line.LocationCode?.SanitizeXMLString(),
                olCust3 = line.ExtendedPrice.ToString(),
                olCust4 = line.LineComment?.CommentText?.SanitizeXMLString(),
                olCust5 = line.UOfM,
                olCust6 = line.CustomerItem?.CustomerItemNumber?.SanitizeXMLString(),
                olCust7 = line.CustomerItem != null ? line.CustomerItem.CustomerItemDescription?.SanitizeXMLString() : string.Empty,
                olCust8 = line.QtyAllocated.ToString(),
                olCust9 = (line.Qty - line.QtyCancelled).ToString(),
                olCust10 = line.QtyInBaseUOfM.ToString(),
                olCust11 = line.CustomerItem?.UserDefined1?.SanitizeXMLString(),
                olCust12 = line.CustomerItem?.UserDefined2?.SanitizeXMLString(),
                olCust13 = line.CustomerItem?.UserDefined3?.SanitizeXMLString(),
                olCust14 = line.CustomerItem?.UserDefined4?.SanitizeXMLString(),
                olCust15 = line.CustomerItem?.UserDefined5?.SanitizeXMLString(),
                olCust16 = line.LineComment?.CommentID?.SanitizeXMLString(),
                olCust29 = line.LineItemSequence.ToString(),
                olCust30 = line.ComponentSequence.ToString(),
                orderLineComments = GetOrderLineComments(line)
            };
        }

        private OrderLineComment[] GetOrderLineComments(SalesTransactionLine line)
        {
            var orderLineComments = new List<OrderLineComment>();
            var priority = 0;
            if (line.LineComment != null)
            {
                if (!string.IsNullOrWhiteSpace(line.LineComment.CommentText))
                {
                    var comments = line.LineComment.CommentText.Trim()?.SanitizeXMLString().Wrap(254);
                    for (int i = 0; i < comments.Length; i++)
                    {
                        orderLineComments.Add(GetOrderLineComment(comments[i]?.SanitizeXMLString(), ++priority));
                    }
                }
            }

            return orderLineComments.ToArray();
        }

        private Order GetOrder(SalesTransaction salesOrder, bool includeLines = true)
        {
            Order order = new Order();
            order.orderId = salesOrder.SopNumber;
            order.orderType = salesOrder.DocumentId;
            order.custId = string.Concat(salesOrder.CustomerNumber, salesOrder.PrimaryShiptoAddressCode);
            order.custCode = salesOrder.CustomerNumber.SanitizeXMLString();
            order.hostId2 = salesOrder.ShippingMethod?.Id?.RemoveSpecialCharacters().SanitizeXMLString(); //Commented out shipmethod and replaced with this according to Roman. 
            order.poNum = salesOrder.CustomerPoNumber;
            order.dateOrdered = salesOrder.DocumentDate;
            order.dateOrderedSpecified = true;
            order.pickCompleteFlag = salesOrder.ShipComplete ? "Y" : "N";
            order.BCompany = salesOrder.CustomerName;
            order.shipByDate = salesOrder.RequestedShipDate;
            order.shipByDateSpecified = true;
            order.shipMethod = salesOrder.ShippingMethod?.Id;


            if (salesOrder.BillToAddress != null)
            {
                var address = salesOrder.BillToAddress;
                order.BAddress1 = address.Address1?.SanitizeXMLString();
                order.BAddress2 = address.Address2?.SanitizeXMLString();
                order.BAddress3 = address.Address3?.SanitizeXMLString();
                order.BCity = address.City?.SanitizeXMLString();
                order.BState = address.State?.SanitizeXMLString();
                order.BZip = address.ZipCode;
                order.BCountry = !string.IsNullOrWhiteSpace(address.CountryCode) ? address.CountryCode : address.Country;
                order.BContact = address.ContactPerson?.SanitizeXMLString();
                order.BPhone = address.Phone;
                order.BFax = address.Fax;
            }
            order.SCompany = salesOrder.ShipToName?.SanitizeXMLString();
            if (salesOrder.ShipToAddress != null)
            {
                var address = salesOrder.ShipToAddress;
                order.SAddress1 = address.Address1?.SanitizeXMLString();
                order.SAddress2 = address.Address2?.SanitizeXMLString();
                order.SAddress3 = address.Address3?.SanitizeXMLString();
                order.SCity = address.City?.SanitizeXMLString();
                order.SState = address.State?.SanitizeXMLString();
                order.SZip = address.ZipCode;
                order.SCountry = !string.IsNullOrWhiteSpace(address.CountryCode) ? address.CountryCode : address.Country;
                order.SContact = address.ContactPerson?.SanitizeXMLString();
                order.SPhone = address.Phone;
                order.SFax = address.Fax;
            }

            //20200619: Added email here as requested by Margie:
            order.emailAddress1Type = "1"; //To
            order.emailAddress1 = DataAccessHelper.GetEmailAddress("CUS", salesOrder.Customer.CustomerNumber, salesOrder.Customer.PrimaryShipToAddressCode);

            UpdateUserDefinedField(order, salesOrder);
            order.orderComments = GetOrderComments(salesOrder);


            if (includeLines)
            {
                //add lines
                order.orderLines = AddOrderLines(order, salesOrder);
            }

            return order;
        }

        private void UpdateUserDefinedField(Order order, SalesTransaction salesOrder)
        {

            order.orCust1 = salesOrder.UserDefined?.UserDefined1?.SanitizeXMLString();
            order.orCust2 = salesOrder.UserDefined?.UserDefined2?.SanitizeXMLString();
            order.orCust3 = salesOrder.UserDefined?.UserDefined3?.SanitizeXMLString();
            order.orCust4 = salesOrder.UserDefined?.UserDefined4?.SanitizeXMLString();
            order.orCust5 = salesOrder.UserDefined?.UserDefined5?.SanitizeXMLString();
            order.orCust6 = salesOrder.UserDefined?.UserDefinedDate1.ToString();
            order.orCust7 = salesOrder.UserDefined?.UserDefinedDate2.ToString();
            order.orCust8 = salesOrder.UserDefined?.UserDefinedTable1?.SanitizeXMLString();
            order.orCust9 = salesOrder.UserDefined?.UserDefinedTable2?.SanitizeXMLString();
            order.orCust10 = salesOrder.UserDefined?.UserDefinedTable3?.SanitizeXMLString();

            order.orCust11 = salesOrder.PrimaryBilltoAddressCode;
            order.orCust12 = salesOrder.PrimaryShiptoAddressCode;

            order.orCust13 = salesOrder.TaxRegistrationNumber?.SanitizeXMLString();
            order.orCust14 = salesOrder.TaxExempt1?.SanitizeXMLString();
            order.orCust15 = salesOrder.TaxExempt2?.SanitizeXMLString();

            order.orCust16 = salesOrder.Customer?.Comment1?.SanitizeXMLString();
            order.orCust17 = salesOrder.Customer?.Comment2?.SanitizeXMLString();

            order.orCust18 = salesOrder.ShippingMethod.Id;

            order.orCust19 = salesOrder.UserDefined?.Comment1?.SanitizeXMLString();
            order.orCust20 = salesOrder.UserDefined?.Comment2?.SanitizeXMLString();
            order.orCust21 = salesOrder.UserDefined?.Comment3?.SanitizeXMLString();
            order.orCust22 = salesOrder.UserDefined?.Comment4?.SanitizeXMLString();

            order.orCust23 = salesOrder.RecordNotes?.TextField?.SanitizeXMLString();

            order.orCust24 = salesOrder.CommentRecord?.Notes?.TextField?.SanitizeXMLString();
        }

        private OrderComment[] GetOrderComments(SalesTransaction order)
        {
            var orderComments = new List<OrderComment>();
            var priority = 0;
            if (order.UserDefined != null)
            {
                if (!string.IsNullOrWhiteSpace(order.UserDefined.CommentText))
                {
                    var comments = order.UserDefined.CommentText.Trim().Wrap(254);
                    for (int i = 0; i < comments.Length; i++)
                    {
                        orderComments.Add(GetOrderComment(comments[i]?.SanitizeXMLString(), ++priority));
                    }
                }
            }


            return orderComments.ToArray();
        }

        private OrderComment GetOrderComment(string commentText, int priority, string commentType = "K") => new OrderComment
        {
            commentType = commentType,
            commentText = commentText,
            priority = priority,
            prioritySpecified = true
        };
        private OrderLineComment GetOrderLineComment(string commentText, int priority, string commentType = "K") => new OrderLineComment
        {
            commentType = commentType,
            commentText = commentText,
            priority = priority,
            prioritySpecified = true
        };

        private short SendDeleteCancelOrder(string action, short sopType, string sopNumber)
        {
            short rtn = 0;
            _sopNumber = sopNumber;
            _sopType = sopType;

            if (string.IsNullOrWhiteSpace(_sopNumber))
                return 2;
            if (_sopType != 2)
                return 2;

            
            var requests = DataAccessHelper.GetSalesOrderRequests(_sopType, _sopNumber);
            if (requests.Count > 0)
            {
                string actionName = action == Constants.PowerhouseIfAction.DELETE ? "DELETE" : "CANCEL";
                var answer = Dynamics.Forms.SyVisualStudioHelper.Functions.DexAsk.Invoke(string.Format("Would you like to send a {0} order(s) request to Powerhouse?", actionName), "Yes", "No", "");
                if (answer == 1)
                {
                    _powerhouseWsSettings = DataAccessHelper.GetPowerhouseWsSettings();
                    if (_powerhouseWsSettings == null)
                    {
                        throw new Exception("Powerhouse Integration setting is missing!");

                    }

                    var salesOrder = DataAccessHelper.GetSalesTransactionWork(_sopType, _sopNumber);
                    if (salesOrder == null)
                    {
                        throw new Exception("Transaction not found!");

                    }

                    var siteId = DataAccessHelper.GetLocationMapping(salesOrder.SiteID);
                    if (string.IsNullOrEmpty(siteId))
                    {
                        throw new Exception("Location mapping not found!");

                    }
                    else
                    {
                        salesOrder.SiteID = siteId;
                    }


                    SendDeleteCancelOrder(requests, action, salesOrder.SiteID);
                }
                else
                    rtn = 2; // Added this so Dex will catch this and won't display any prompt. 
            }
            return rtn;
        }

        private void SendDeleteCancelOrder(List<SalesOrderLineTransferRequest> requests, string action, string siteId)
        {
            var client = new PHWebServices();
            client.Url = _powerhouseWsSettings.Url;
            string sessionId = null;
            try
            {
                //connect
                sessionId = client.reLogIn(null, _powerhouseWsSettings.PowerhouseUserId, _powerhouseWsSettings.PowerhousePassword);
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    throw new Exception("Cannot Establish a connection to Powerhouse Webservices.");
                }
                List<Order> orders = new List<Order>();
                foreach (var request in requests)
                {
                    if (!orders.Any(o => o.orderId == request.SopNumber && o.releaseNum == request.VersionNumber))
                    {
                        orders.Add(new Order
                        {
                            ifActionId = action,
                            warehouseId = siteId,
                            ownerId = _powerhouseWsSettings.OwnerId,
                            orderId = request.SopNumber,
                            releaseNum = "0"
                        });
                    }
                }
                var responses = client.sendOrdersTran(sessionId, orders.ToArray());
                if (responses.Any(r => !string.IsNullOrWhiteSpace(r.ifErrorMsg)))
                {
                    var errors = new List<string>();
                    foreach (var response in responses)
                    {
                        if (!string.IsNullOrWhiteSpace(response.ifErrorMsg))
                        {
                            errors.Add(response.ifErrorMsg);
                        }
                    }
                    if (errors.Count > 0)
                    {
                        throw new Exception(string.Format("An error occured while trying to process your request. Powerhouse response message: {0}", string.Join("\n", errors)));
                    }
                }

                foreach (var o in orders)
                {
                    //clear previous and log
                    DataAccessHelper.UpdateSalesOrderRequests(o);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (client != null)
                {
                    try
                    {
                        client.logOut(sessionId);
                    }
                    catch
                    {
                        //don't care about this exception, if happened.
                    }
                }
            }
        }

        private void BspTriggerFunctionDeleteSopEntryProc_InvokeAfterOriginal(object sender, BspDictionary.BspTriggerFunctionDeleteSopEntryProcProcedure.InvokeEventArgs e)
        {
            try
            {
                e.outParam3 = SendDeleteCancelOrder(Constants.PowerhouseIfAction.DELETE, e.inParam1, e.inParam2);
            }
            catch(Exception ex)
            {
                e.outParam3 = 1;
                e.outParam4 = ex.Message;
            }
        }

        private void BspTriggerFunctionVoidSopEntryProc_InvokeAfterOriginal(object sender, BspDictionary.BspTriggerFunctionVoidSopEntryProcProcedure.InvokeEventArgs e)
        {
            try
            {
                e.outParam3 = SendDeleteCancelOrder(Constants.PowerhouseIfAction.CANCEL, e.inParam1, e.inParam2);
            }
            catch (Exception ex)
            {
                e.outParam3 = 1;
                e.outParam4 = ex.Message;
            }
        }

        #endregion

        #region Item

        private void InventoryItemSendToPowerhouse_InvokeAfterOriginal(object sender, BspDictionary.BspIvItemMaintenanceIntegrationForm.SendToPowerhouseProcedure.InvokeEventArgs e)
        {
            _itemNumber = e.inParam1;
            try
            {
                _powerhouseWsSettings = DataAccessHelper.GetPowerhouseWsSettings();
                if (_powerhouseWsSettings == null)
                {
                    throw new Exception("Powerhouse Integration setting is missing!");
                }
                var item = DataAccessHelper.GetItemMaster(_itemNumber);
                if (item == null)
                {
                    throw new Exception("Record not found!");
                }

                var response = SendItem(item);
                if (response != null)
                {
                    if (!string.IsNullOrWhiteSpace(response.ifErrorMsg))
                    {
                        throw new Exception(string.Format("An error occured while trying to process your request. Powerhouse response message: {0}", response.ifErrorMsg));

                    }
                    DataAccessHelper.LogItemTransfer(_itemNumber, response.ifStatus, response.ifErrorMsg);
                }
                else
                {
                    throw new Exception("Response Returned Null");
                }
            }
            catch (Exception ex)
            {
                DataAccessHelper.LogItemTransfer(_itemNumber, "E", ex.Message);

                e.outParam2 = 1;
                e.outParam3 = ex.Message;
            }
        }

        private void IVItemMaintWindowSaveButton_ClickAfterOriginal(object sender, EventArgs e)
        {
            _itemNumber = IvItemMaintenanceForm.IvItemMaintenance.ItemNumber.Value;
            var itemType = IvItemMaintenanceForm.IvItemMaintenance.ItemType.Value;
            if (!string.IsNullOrWhiteSpace(_itemNumber) && itemType == 1 && IvItemMaintenanceForm.IvItemMaintenance.IsChanged)
            {
                var answer = Dynamics.Forms.SyVisualStudioHelper.Functions.DexAsk.Invoke("Do you want to send this item to Powerhouse?", "Yes", "No", "");
                if (answer == 1)
                {
                    BspIvItemMaintenanceIntegrationForm.Open();
                    BspIvItemMaintenanceIntegrationForm.BspIvItemMaintenanceIntegration.ItemNumber.Value = _itemNumber;
                }
            }
        }


        private ItemMasterResponse SendItem(Models.ItemMaster item)
        {
            var client = new PHWebServices();
            client.Url = _powerhouseWsSettings.Url;
            string sessionId = null;
            try
            {
                //connect
                sessionId = client.reLogIn(null, _powerhouseWsSettings.PowerhouseUserId, _powerhouseWsSettings.PowerhousePassword);
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    Dynamics.Forms.SyVisualStudioHelper.Functions.DexWarning.Invoke("Cannot Establish a connection to Powerhouse Webservices.");
                    return null;
                }
                //var bspItemMaster = DataAccessHelper.GetBspItemMaster(item.ItemNumber);

                var phsItem = new PowerhouseWS.ItemMaster
                {
                    ifActionId = Constants.PowerhouseIfAction.RESET,
                    ownerId = _powerhouseWsSettings.OwnerId,
                    itemId = item.ItemNumber,
                    description = item.ItemDescription,
                    itemClass = item.ItemClassCode,
                    harmonizedCode = item.UserCategoryValues?.ElementAtOrDefault(2),
                    lotCtrlFlag = item.ItemTrackingOption == 3 ? "Y" : "N",
                    serialCtrlFlag = item.ItemTrackingOption == 2 ? "Y" : "N",
                    imCust1 = item.UserCategoryValues?.ElementAtOrDefault(0) ?? string.Empty,
                    imCust4 = item.UserCategoryValues?.ElementAtOrDefault(1) ?? string.Empty,
                    imCust5 = item.UserCategoryValues?.ElementAtOrDefault(2) ?? string.Empty,
                    imCust6 = item.UserCategoryValues?.ElementAtOrDefault(3) ?? string.Empty,
                    imCust7 = item.UserCategoryValues?.ElementAtOrDefault(4) ?? string.Empty,
                    imCust8 = item.UserCategoryValues?.ElementAtOrDefault(5) ?? string.Empty,
                    imCust9 = item.AlternateItem1,
                    imCust10 = item.AlternateItem2,
                    markForOsFlag = "Y",
                    hostTrackingFlag = "Y",
                    caseConveyFlag = "N",
                    autoLabelFlag = "N",
                };

                return client.sendItem(sessionId, phsItem);
            }
            catch (Exception ex)
            {
                Dynamics.Forms.SyVisualStudioHelper.Functions.DexWarning.Invoke(string.Format("An error occured while trying to send the request. error details: {0}", ex.Message));
                return null;
            }
            finally
            {
                if (client != null)
                {
                    try
                    {
                        client.logOut(sessionId);
                    }
                    catch
                    {
                        //don't care about this exception, if happened.
                    }
                }
            }
        }
        #endregion

        #region Purchase Order Methods

        private void ReceivingSendToPowerhouse_InvokeAfterOriginal(object sender, BspDictionary.BspPopReceivingIntegrationForm.SendToPowerhouseProcedure.InvokeEventArgs e)
        {
            _containerID = e.inParam1;

            try
            {
                _powerhouseWsSettings = DataAccessHelper.GetPowerhouseWsSettings();
                if (_powerhouseWsSettings == null)
                {
                    throw new Exception("Powerhouse Integration setting is missing!");
                }

                var recevingTrx = DataAccessHelper.GetReceiving(_containerID);
                if (recevingTrx == null)
                {
                    throw new Exception("Transaction not found!");
                }

                var response = SendReceipt(recevingTrx);
                if (response != null)
                {
                    if (!string.IsNullOrWhiteSpace(response.ifErrorMsg))
                    {
                        //log Message
                        DataAccessHelper.LogReceiptTransfer(_containerID, response.ifStatus, response.ifErrorMsg);
                        throw new Exception(string.Format("An error occured while trying to process your request. Powerhouse response message: {0}", response.ifErrorMsg));
                    }
                    DataAccessHelper.LogReceiptTransfer(_containerID, response.ifStatus, response.ifErrorMsg);
                }
                else
                {
                    DataAccessHelper.LogReceiptTransfer(_containerID, "E", "Response Returned Null");
                    throw new Exception("Response Returned Null");
                }
            }
            catch (Exception ex)
            {
                e.outParam2 = 1;
                e.outParam3 = ex.Message;
            }
        }

        private ReceiptResponse SendReceipt(ReceiptTransfer receiptTransfer)
        {
            var client = new PHWebServices();
            client.Url = _powerhouseWsSettings.Url;
            string sessionId = null;
            try
            {
                //connect
                sessionId = client.reLogIn(null, _powerhouseWsSettings.PowerhouseUserId, _powerhouseWsSettings.PowerhousePassword);
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    Dynamics.Forms.SyVisualStudioHelper.Functions.DexWarning.Invoke("Cannot Establish a connection to Powerhouse Webservices.");
                    return null;
                }
                Receipt receipt = new Receipt();
                receipt.warehouseId = receiptTransfer.LocationCode;
                receipt.ownerId = _powerhouseWsSettings.OwnerId;
                receipt.po = receiptTransfer.ContainerID;
                receipt.releaseNum = "0";
                receipt.createdUsrId = Dynamics.Globals.UserId.Value;
                receipt.createdDate = receiptTransfer.CreatedDate;
                receipt.createdDateSpecified = true;
                receipt.receiptType = _powerhouseWsSettings.ContainerReceivingType; // CONTR

                // Added as per Margie's request 09122020
                receipt.dateExpected = receiptTransfer.ActualShip;
                receipt.dateExpectedSpecified = true;

                var receiptLines = new List<ReceiptLine>();
                foreach (var item in receiptTransfer.Items)
                {
                    if (item.ShouldBeSentToPowerHouse)
                    {
                        var receiptLine = new ReceiptLine
                        {
                            warehouseId = receipt.warehouseId,
                            ownerId = receipt.ownerId,
                            po = receipt.po,
                            releaseNum = receipt.releaseNum,
                            receiptLine = item.LineItemSequence,
                            receiptLineSpecified = true,
                            itemId = item.ItemNumber,
                            usrUom = item.UofM,
                            piecesOrdered = Convert.ToDouble(item.QTYShipped * item.UofMQtyInBase),
                            piecesOrderedSpecified = true,
                            piecesExpected = Convert.ToDouble(item.QTYShipped * item.UofMQtyInBase),
                            piecesExpectedSpecified = true,
                            vendorItemId = item.VendorItemNumber,
                            glAccount = item.InventoryAccount,
                            hostLine = item.ReceiptLineItem.ToString(),
                            dateExpected = item.PromisedDate,
                            dateExpectedSpecified = true,
                            rlCust1 = item.ReceiptNumber,
                            rlCust2 = item.LocationCode,
                            rlCust3 = item.PoNumber,
                            rlCust4 = item.ItemDesc,
                            rlCust5 = item.VendorItemNumber,
                            rlCust6 = item.VendorItemDescription,
                            rlCust7 = item.ActualShipDate.ToShortDateString(),
                            rlCust8 = item.ShipMethod,
                            rlCust10 = item.UofMQtyInBase.ToString(),
                        };
                        receiptLines.Add(receiptLine);
                    }

                }
                receipt.receiptLines = receiptLines.ToArray();

                var response = client.sendReceipt(sessionId, receipt);
                if (response != null)
                {
                    if (!string.IsNullOrWhiteSpace(response.ifErrorMsg))
                    {
                        return response;
                    }
                    DataAccessHelper.UpdatePORequests(receipt);
                    //clear previous and logDataAccessHelper.UpdateSalesOrderRequests(order);

                }
                else
                    throw new Exception("Powerhouse integration was unsuccesful.");

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (client != null)
                {
                    try
                    {
                        client.logOut(sessionId);
                    }
                    catch
                    {
                        //don't care about this exception, if happened.
                    }
                }
            }
        }

        private void ReceivingDeleteRecord_InvokeAfterOriginal(object sender, BspDictionary.BspPopReceivingIntegrationMaintenanceForm.DeleteRecordProcedure.InvokeEventArgs e)
        {
            _containerID = e.inParam1;

            _powerhouseWsSettings = DataAccessHelper.GetPowerhouseWsSettings();
            if (_powerhouseWsSettings == null)
            {
                throw new Exception("Powerhouse Integration setting is missing!");
            }

            var receiving = DataAccessHelper.GetReceiving(_containerID);
            if (receiving == null)
            {
                throw new Exception ("Transaction not found!");
            }

            try
            {
                var response = SendDeleteReceiving(receiving);
                if (response != null)
                {
                    if (!string.IsNullOrWhiteSpace(response.ifErrorMsg))
                    {
                        throw new Exception(string.Format("An error occured while trying to process your request. Powerhouse response message: {0}", response.ifErrorMsg));
                    }
                    else
                    {
                        e.outParam2 = 0;
                        e.outParam3 = string.Empty;
                    }
                }
                else
                {
                    throw new Exception("Response Returned Null");
                }
            }
            catch (Exception ex)
            {
                e.outParam2 = 1;
                e.outParam3 = ex.Message;
            }
        }

        private ReceiptResponse SendDeleteReceiving(ReceiptTransfer receiving)
        {
            var client = new PHWebServices();
            client.Url = _powerhouseWsSettings.Url;
            string sessionId = null;
            try
            {
                //connect
                sessionId = client.reLogIn(null, _powerhouseWsSettings.PowerhouseUserId, _powerhouseWsSettings.PowerhousePassword);
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    throw new Exception("Cannot Establish a connection to Powerhouse Webservices.");
                }
                var receipt = new Receipt
                {
                    ifActionId = PowerHouse.Constants.PowerhouseIfAction.DELETE,
                    warehouseId = receiving.LocationCode,
                    ownerId = _powerhouseWsSettings.OwnerId,
                    po = receiving.ContainerID,
                    releaseNum = "0"
                };

                var response = client.sendReceipt(sessionId, receipt);
                if (string.IsNullOrEmpty(response.ifErrorMsg))
                    DataAccessHelper.DeletePORequests(receipt);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (client != null)
                {
                    try
                    {
                        client.logOut(sessionId);
                    }
                    catch
                    {
                        //don't care about this exception, if happened.
                    }
                }
            }
        }

        private void POSendToPowerhouse_InvokeAfterOriginal(object sender, BspDictionary.BspPopPoEntryIntegrationForm.SendToPowerhouseProcedure.InvokeEventArgs e)
        {

            _poNumber = e.inParam1;
            try
            {
                _powerhouseWsSettings = DataAccessHelper.GetPowerhouseWsSettings();
                if (_powerhouseWsSettings == null)
                {
                    throw new Exception("Powerhouse Integration setting is missing!");
                }
                var purchaseOrder = DataAccessHelper.GetPurchaseOrder(_poNumber);
                if (purchaseOrder == null)
                {
                    throw new Exception("Transaction not found!");
                }
                if (purchaseOrder.PoStatus > 3 && purchaseOrder.Items.Any(i => i.QtyRemainingToShip > 0))
                {
                    throw new Exception("This purchase order has been Received, Canceled, or Closed");
                }

                var response = SendPurchaseOrder(purchaseOrder);
                if (response != null)
                {
                    if (!string.IsNullOrWhiteSpace(response.ifErrorMsg))
                    {
                        DataAccessHelper.LogPOTransfer(_poNumber, response.ifStatus, response.ifErrorMsg);
                        throw new Exception(string.Format("An error occured while trying to process your request. Powerhouse response message: {0}", response.ifErrorMsg));

                    }
                    DataAccessHelper.LogPOTransfer(_poNumber, response.ifStatus, response.ifErrorMsg);
                }
                else
                {
                    DataAccessHelper.LogPOTransfer(_poNumber, "E", "Response Returned Null");
                    throw new Exception("Response Returned Null");
                }
            }
            catch (Exception ex)
            {
                e.outParam2 = 1;
                e.outParam3 = ex.Message;
            }
        }

        private ReceiptResponse SendPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            var client = new PHWebServices();
            client.Url = _powerhouseWsSettings.Url;
            string sessionId = null;
            try
            {
                //connect
                sessionId = client.reLogIn(null, _powerhouseWsSettings.PowerhouseUserId, _powerhouseWsSettings.PowerhousePassword);
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    throw new Exception("Cannot Establish a connection to Powerhouse Webservices.");
                    
                }
                Receipt receipt = new Receipt();

                receipt.ifActionId = Constants.PowerhouseIfAction.RESET;

                receipt.warehouseId = purchaseOrder.LocationCode;
                receipt.ownerId = _powerhouseWsSettings.OwnerId;
                receipt.po = purchaseOrder.PoNumber;
                //check this entry
                receipt.releaseNum = purchaseOrder.RevisionNumber.ToString();
                receipt.createdUsrId = Dynamics.Globals.UserId.Value;
                receipt.createdDate = purchaseOrder.CreatedDate;
                receipt.createdDateSpecified = true;
                receipt.receiptType = _powerhouseWsSettings.POReceivingType; 
                receipt.vendorId = purchaseOrder.VendorId;
                receipt.BCompany = purchaseOrder.PurchaseCompanyName;
                receipt.BAddress1 = purchaseOrder.PurchaseAddress1;
                receipt.BAddress2 = purchaseOrder.PurchaseAddress2;
                receipt.BAddress3 = purchaseOrder.PurchaseAddress3;
                receipt.BCity = purchaseOrder.PurchaseCity;
                receipt.BState = purchaseOrder.PurchaseState;
                receipt.BZip = purchaseOrder.PurchaseZipCode;
                receipt.BCountry = purchaseOrder.PurchaseCountry;
                receipt.BContact = purchaseOrder.PurchaseContact;
                receipt.BPhone = purchaseOrder.PurchasePhone1;
                receipt.BFax = purchaseOrder.PurchaseFax;

                receipt.SCompany = purchaseOrder.PurchaseCompanyName;
                receipt.SAddress1 = purchaseOrder.PurchaseAddress1;
                receipt.SAddress2 = purchaseOrder.PurchaseAddress2;
                receipt.SAddress3 = purchaseOrder.PurchaseAddress3;
                receipt.SCity = purchaseOrder.PurchaseCity;
                receipt.SState = purchaseOrder.PurchaseState;
                receipt.SZip = purchaseOrder.PurchaseZipCode;
                receipt.SCountry = purchaseOrder.PurchaseCountry;
                receipt.SContact = purchaseOrder.PurchaseContact;
                receipt.SPhone = purchaseOrder.PurchasePhone1;
                receipt.SFax = purchaseOrder.PurchaseFax;


                receipt.dateExpected = purchaseOrder.PromisedDate;
                receipt.dateExpectedSpecified = true;

                var receiptLines = new List<ReceiptLine>();
                foreach (var item in purchaseOrder.Items)
                {
                    if (item.ShouldBeSentToPowerHouse)
                    {
                        var receiptLine = new ReceiptLine
                        {
                            warehouseId = receipt.warehouseId,
                            ownerId = receipt.ownerId,
                            po = item.PoNumber,
                            releaseNum = receipt.releaseNum,
                            receiptLine = item.Ord,
                            receiptLineSpecified = true,
                            itemId = item.ItemNumber,
                            //usrUom = item.UOfM,
                            piecesOrdered = Convert.ToDouble((item.QtyOrdered - item.QtyCanceled) * item.UOfMQtyInBase),
                            piecesOrderedSpecified = true,
                            piecesExpected = Convert.ToDouble(item.QtyRemainingToShip * item.UOfMQtyInBase),
                            piecesExpectedSpecified = true,
                            vendorItemId = item.VendorItemNumber,
                            glAccount = item.InventoryAccountNumber,
                            hostLine = item.Ord.ToString(),
                            dateExpected = item.PromisedDate,
                            dateExpectedSpecified = true,
                            vendorId = item.VendorId,
                            rlCust2 = item.LocationCode,
                            rlCust10 = item.UOfMQtyInBase.ToString(),
                        };
                        receiptLines.Add(receiptLine);
                    }
                }
                receipt.receiptLines = receiptLines.ToArray();

                return client.sendReceipt(sessionId, receipt);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error occured while trying to send the request. error details: {0}", ex.Message));
            }
            finally
            {
                if (client != null)
                {
                    try
                    {
                        client.logOut(sessionId);
                    }
                    catch
                    {
                        //don't care about this exception, if happened.
                    }
                }
            }
        }

        #endregion

#if DEBUG
        private void LogResult<T>(T obj, string fileName = "result")
        {
            var writer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            var path = AppDomain.CurrentDomain.BaseDirectory + "//TraceLog//" + fileName + "_" + DateTime.Now.Ticks + ".xml";
            System.IO.FileStream file = System.IO.File.Create(path);

            writer.Serialize(file, obj);
            file.Close();
        }
#endif
    }
}
