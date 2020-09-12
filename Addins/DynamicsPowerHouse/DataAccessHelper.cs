using System;
using System.Collections.Generic;
using System.Linq;
using BSP.DynamicsGP.PowerHouse.Models;
using Microsoft.Dexterity.Bridge;
using Microsoft.Dexterity.Applications;
using Microsoft.Dexterity.Applications.DynamicsDictionary;
using BSP.DynamicsGP.PowerHouse.Extensions;

namespace BSP.DynamicsGP.PowerHouse
{
    public static class DataAccessHelper
    {

        public static PowerhouseWsSetting GetPowerhouseWsSettings()
        {
            TableError lastError;

            var powerhouseSetupTable = BusinessSolutionPartners.Tables.BspIntegrationSetp;
            PowerhouseWsSetting setting;

            try
            {
                powerhouseSetupTable.Key = 1;

                powerhouseSetupTable.CompanyId.Value = Dynamics.Globals.CompanyId.Value;
                lastError = powerhouseSetupTable.Get();
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound)
                    {
                        throw new Exception(lastError.ToString());
                    }
                }

                setting = new PowerhouseWsSetting
                {
                    Url = powerhouseSetupTable.Url.Value,
                    PowerhouseUserId = powerhouseSetupTable.UserId.Value,
                    PowerhousePassword = powerhouseSetupTable.BspPowerhousePassword.Value,
                    OwnerId = powerhouseSetupTable.Owner.Value,
                    SOQtyToUse = powerhouseSetupTable.BspSoLineQtyToUse.Value, //1 = Order, 2 = Allocated
                    SalesBatchID = powerhouseSetupTable.BspSopBatchId.Value,
                    POReceivingType = powerhouseSetupTable.BspPhpoReceivingType.Value,
                    ContainerReceivingType = powerhouseSetupTable.BspPhContainerReceivingType.Value
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                powerhouseSetupTable.Close();
            }
            return setting;
        }

        #region Item

        public static ItemMaster GetItemMaster(string itemNumber)
        {
            TableError lastError;
            ItemMaster item;

            var ItemMstrTable = Dynamics.Tables.IvItemMstr;
            try
            {
                // Set the key to use for the table
                ItemMstrTable.Key = 1;

                ItemMstrTable.ItemNumber.Value = itemNumber;

                lastError = ItemMstrTable.Get();
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound)
                    {
                        throw new Exception(lastError.ToString());
                    }
                    ItemMstrTable.Close();
                    return null;
                }

                item = new ItemMaster()
                {
                    ItemNumber = ItemMstrTable.ItemNumber.Value,
                    ItemDescription = ItemMstrTable.ItemDescription.Value,
                    ItemType = ItemMstrTable.ItemType.Value,
                    //ItemClassCode = ItemMstrTable.ItemClassCode.Value,
                    ItemClassCode = GetItemClassCode(ItemMstrTable.ItemNumber.Value),
                    ItemTrackingOption = ItemMstrTable.ItemTrackingOption.Value,
                    AlternateItem1 = ItemMstrTable.AlternateItem1.Value,
                    AlternateItem2 = ItemMstrTable.AlternateItem2.Value,

                    UserCategoryValues = ItemMstrTable.UserCategoryValues.Value.ToList(),

                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                ItemMstrTable.Close();
            }
            return item;
        }

        private static string GetItemClassCode(string itemNumber)
        {
            TableError lastError;
            string itemClassCode = string.Empty;

            var itemExtVTable  = BusinessSolutionPartners.Tables.BspPowerhouseItemExtV;
            try
            {
                itemExtVTable.Release();
                itemExtVTable.RangeClear();

                // Set the key to use for the table
                itemExtVTable.Key = 1;

                itemExtVTable.ItemNumber.Value = itemNumber;

                lastError = itemExtVTable.Get();
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound)
                    {
                        throw new Exception(lastError.ToString());
                    }
                }

                itemClassCode = itemExtVTable.BspProductClass.Value; 
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                itemExtVTable.Close();
            }
            return itemClassCode;
        }

        #endregion

        #region Sales Order

        public static SalesTransaction GetSalesTransactionWork(short sopType, string sopNumber)
        {
            TableError lastError;
            SalesTransaction salesTrx;

            var SOPHeaderWorkTable = Dynamics.Tables.SopHdrWork;
            var SOPLineWorkTable = Dynamics.Tables.SopLineWork;

            // Set the key to use for the table
            // SOP Number, SOP Type
            SOPHeaderWorkTable.Key = 1;

            SOPHeaderWorkTable.SopType.Value = sopType;
            SOPHeaderWorkTable.SopNumber.Value = sopNumber;
            try
            {
                // Read through all of the rows in the range
                lastError = SOPHeaderWorkTable.Get();
            
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound && !Constants.SuppressUserErrorNotification)
                    {
                        throw new Exception(lastError.ToString());
                    }
                }

                salesTrx = new SalesTransaction();
                salesTrx.SopNumber = SOPHeaderWorkTable.SopNumber.Value;
                salesTrx.SopType = SOPHeaderWorkTable.SopType.Value;
                salesTrx.DocumentId = SOPHeaderWorkTable.DocumentId.Value;
                salesTrx.OriginalNumber = SOPHeaderWorkTable.OriginalNumber.Value;
                salesTrx.OriginalType = SOPHeaderWorkTable.OriginalType.Value;
                salesTrx.CustomerName = SOPHeaderWorkTable.CustomerName.Value;
                salesTrx.CustomerNumber = SOPHeaderWorkTable.CustomerNumber.Value;
                salesTrx.CustomerPoNumber = SOPHeaderWorkTable.CustomerPoNumber.Value;
                salesTrx.DocumentDate = SOPHeaderWorkTable.DocumentDate.Value;
                salesTrx.RequestedShipDate = SOPHeaderWorkTable.RequestedShipDate.Value;
                salesTrx.SalespersonId = SOPHeaderWorkTable.SalespersonId.Value;
                salesTrx.ShippingMethod = GetShippingMethod(SOPHeaderWorkTable.ShippingMethod.Value);
                salesTrx.ShipToName = SOPHeaderWorkTable.ShipToName.Value;
                salesTrx.SiteID = SOPHeaderWorkTable.LocationCode.Value;
                salesTrx.Subtotal = SOPHeaderWorkTable.Subtotal.Value;
                salesTrx.ShipComplete = SOPHeaderWorkTable.ShipCompleteDocument.Value;
                salesTrx.CreatedDate = SOPHeaderWorkTable.CreatedDate.Value;
                salesTrx.ModifiedDate = SOPHeaderWorkTable.ModifiedDate.Value;
                salesTrx.TaxRegistrationNumber = SOPHeaderWorkTable.TaxRegistrationNumber.Value;
                salesTrx.TaxExempt1 = SOPHeaderWorkTable.TaxExempt1.Value;
                salesTrx.TaxExempt2 = SOPHeaderWorkTable.TaxExempt2.Value;
                salesTrx.UserDefined = GetSalesUserDefinedWorkHistory(SOPHeaderWorkTable.SopType.Value, SOPHeaderWorkTable.SopNumber.Value);
                salesTrx.PrimaryShiptoAddressCode = SOPHeaderWorkTable.PrimaryShiptoAddressCode.Value;
                salesTrx.PrimaryBilltoAddressCode = SOPHeaderWorkTable.PrimaryBilltoAddressCode.Value;
                salesTrx.Customer = GetCustomer(SOPHeaderWorkTable.CustomerNumber.Value);
                salesTrx.CommentRecord = GetCommentRecord(SOPHeaderWorkTable.CommentId.Value);
                salesTrx.RecordNotes = GetRecordNotes(SOPHeaderWorkTable.NoteIndex.Value);
                salesTrx.BatchNumber = SOPHeaderWorkTable.BatchNumber.Value;
                salesTrx.ShipToAddress = new Address()
                {
                    ContactPerson = SOPHeaderWorkTable.ContactPerson.Value,
                    Address1 = SOPHeaderWorkTable.Address1.Value,
                    Address2 = SOPHeaderWorkTable.Address2.Value,
                    Address3 = SOPHeaderWorkTable.Address3.Value,
                    City = SOPHeaderWorkTable.City.Value,
                    State = SOPHeaderWorkTable.State.Value,
                    ZipCode = SOPHeaderWorkTable.ZipCode.Value,
                    Country = SOPHeaderWorkTable.Country.Value,
                    CountryCode = SOPHeaderWorkTable.CountryCode.Value,
                    Phone = SOPHeaderWorkTable.PhoneNumber1.Value,
                    Fax = SOPHeaderWorkTable.FaxNumber.Value,
                    Email = GetEmailAddress("CUS", SOPHeaderWorkTable.CustomerNumber.Value, SOPHeaderWorkTable.PrimaryShiptoAddressCode)
                };

                salesTrx.BillToAddress = GetCustomerAddress(SOPHeaderWorkTable.CustomerNumber, SOPHeaderWorkTable.PrimaryBilltoAddressCode.Value);
                // Specify the range for the table
                // Start of the range
                SOPLineWorkTable.Clear();
                SOPLineWorkTable.SopType.Value = sopType;
                SOPLineWorkTable.SopNumber.Value = sopNumber;
                SOPLineWorkTable.RangeStart();

                // End of the range
                SOPLineWorkTable.Fill();
                SOPLineWorkTable.SopType.Value = sopType;
                SOPLineWorkTable.SopNumber.Value = sopNumber;
                SOPLineWorkTable.RangeEnd();

                // Read through all of the rows in the range
                lastError = SOPLineWorkTable.GetFirst();
                while (lastError == TableError.NoError)
                {
                    var itemMaster = GetItemMaster(SOPLineWorkTable.ItemNumber);
                    salesTrx.Items.Add(new SalesTransactionLine()
                    {
                        SopNumber = SOPLineWorkTable.SopNumber.Value,
                        SopType = SOPLineWorkTable.SopType.Value,
                        ComponentSequence = SOPLineWorkTable.ComponentSequence.Value,
                        LineItemSequence = SOPLineWorkTable.LineItemSequence.Value,
                        ItemNumber = SOPLineWorkTable.ItemNumber.Value,
                        ItemDescription = SOPLineWorkTable.ItemDescription.Value,
                        ItemType = Convert.ToInt32(itemMaster?.ItemType),
                        RequestedShipDate = SOPLineWorkTable.RequestedShipDate.Value,
                        QtyToInvoice = SOPLineWorkTable.QtyToInvoice.Value,
                        UOfM = SOPLineWorkTable.UOfM.Value,
                        ExtendedPrice = SOPLineWorkTable.ExtendedPrice.Value,
                        UnitPrice = SOPLineWorkTable.UnitPrice.Value,
                        QtyFulfilled = SOPLineWorkTable.QtyFulfilled.Value,
                        QtyInBaseUOfM = SOPLineWorkTable.QtyInBaseUOfM.Value,
                        QtyRemaining = SOPLineWorkTable.QtyRemaining.Value,
                        Qty = SOPLineWorkTable.Qty.Value,
                        QtyOrdered = SOPLineWorkTable.QtyOrdered.Value,
                        QtyAllocated = SOPLineWorkTable.QtyAllocated.Value,
                        QtyCancelled = SOPLineWorkTable.QtyCanceled.Value,
                        QtyToShip = SOPLineWorkTable.QtyToShip.Value,
                        QtyToBackOrder = SOPLineWorkTable.QtyToBackOrder.Value,
                        QtyPrevInvoiced = SOPLineWorkTable.QtyPrevInvoiced.Value,
                        LocationCode = SOPLineWorkTable.LocationCode.Value,
                        NonIv = SOPLineWorkTable.NonIv.Value,
                        DropShip = SOPLineWorkTable.DropShip.Value,
                        CommodityCode = itemMaster?.UserCategoryValues?.ElementAtOrDefault(2),
                        CustomerItem = GetCustomerItemXref(SOPLineWorkTable.ItemNumber.Value, SOPHeaderWorkTable.CustomerNumber.Value),
                        TransferRequest = GetSalesLineTransferRequest(SOPLineWorkTable.SopType.Value, SOPLineWorkTable.SopNumber.Value, SOPLineWorkTable.ComponentSequence.Value, SOPLineWorkTable.LineItemSequence.Value),
                        LineComment = GetCommentRecord(SOPLineWorkTable.CommentId.Value)
                    });
                    // Get the next line item
                    lastError = SOPLineWorkTable.GetNext();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                // Close the table
                SOPHeaderWorkTable.Close();
                SOPLineWorkTable.Close();
            }
            return salesTrx;
        }

        private static SalesUserDefined GetSalesUserDefinedWorkHistory(short sopType, string sopNumber)
        {
            TableError lastError;

            SalesUserDefined userDef;

            var UsrDefWorkHistTable = Dynamics.Tables.SopUsrDefWorkHist;
            try
            {
                // Set the key to use for the table
                // SOP Number, SOP Type
                UsrDefWorkHistTable.Key = 1;

                UsrDefWorkHistTable.SopType.Value = sopType;
                UsrDefWorkHistTable.SopNumber.Value = sopNumber;

                // Read through all of the rows in the range
                lastError = UsrDefWorkHistTable.Get();
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound)
                    {
                        throw new Exception(lastError.ToString());
                    }
                }
                
                userDef = new SalesUserDefined()
                {
                    Comment = UsrDefWorkHistTable.Comment != null ? UsrDefWorkHistTable.Comment.Value.ToList() : null,
                    Comment1 = UsrDefWorkHistTable.Comment != null && UsrDefWorkHistTable.Comment[0] != null ? UsrDefWorkHistTable.Comment[0]?.Value : null,
                    Comment2 = UsrDefWorkHistTable.Comment != null && UsrDefWorkHistTable.Comment[1] != null ? UsrDefWorkHistTable.Comment[1]?.Value : null,
                    Comment3 = UsrDefWorkHistTable.Comment != null && UsrDefWorkHistTable.Comment[2] != null ? UsrDefWorkHistTable.Comment[2]?.Value : null,
                    Comment4 = UsrDefWorkHistTable.Comment != null && UsrDefWorkHistTable.Comment[3] != null ? UsrDefWorkHistTable.Comment[3]?.Value : null,
                    CommentText = TrimAndRemoveCRLFFromString(UsrDefWorkHistTable.CommentText?.Value),
                    SopNumber = UsrDefWorkHistTable.SopNumber?.Value,
                    SopType = 2,
                    UserDefined1 = UsrDefWorkHistTable.UserDefined1?.Value,
                    UserDefined2 = UsrDefWorkHistTable.UserDefined2?.Value,
                    UserDefined3 = UsrDefWorkHistTable.UserDefined3?.Value,
                    UserDefined4 = UsrDefWorkHistTable.UserDefined4?.Value,
                    UserDefined5 = UsrDefWorkHistTable.UserDefined5?.Value,
                    UserDefinedDate1 = UsrDefWorkHistTable.UserDefinedDate1 != null ? UsrDefWorkHistTable.UserDefinedDate1.Value : DateTime.MinValue,
                    UserDefinedDate2 = UsrDefWorkHistTable.UserDefinedDate2 != null ? UsrDefWorkHistTable.UserDefinedDate2.Value : DateTime.MinValue,
                    UserDefinedTable1 = UsrDefWorkHistTable.UserDefinedTable1?.Value,
                    UserDefinedTable2 = UsrDefWorkHistTable.UserDefinedTable2?.Value,
                    UserDefinedTable3 = UsrDefWorkHistTable.UserDefinedTable3?.Value,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                UsrDefWorkHistTable.Close();
            }
            return userDef;
        }

        private static ShippingMethod GetShippingMethod(string shippingMethodId)
        {
            TableError lastError;
            ShippingMethod shippingMethod;

            var ShippingMethodMstrTable = Dynamics.Tables.SyShippingMethodsMstr;

            // Set the key to use for the table
            ShippingMethodMstrTable.Key = 1;

            ShippingMethodMstrTable.ShippingMethod.Value = shippingMethodId;

            try
            {
                lastError = ShippingMethodMstrTable.Get();
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound && !Constants.SuppressUserErrorNotification)
                    {
                        throw new Exception(lastError.ToString());
                    }
                }

                shippingMethod = new ShippingMethod()
                {
                    Id = ShippingMethodMstrTable.ShippingMethod.Value,
                    Description = ShippingMethodMstrTable.ShippingMethodDescription.Value
                };
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                ShippingMethodMstrTable.Close();
            }


            return shippingMethod;
        }

        public static string GetEmailAddress(string masterType, string masterId, string addressCode)
        {
            TableError lastError;
            var coInetAddressTable = Dynamics.Tables.CoINetAddrs;
            string emailAddress;

            coInetAddressTable.Key = 1;
            coInetAddressTable.MasterType.Value = masterType;
            coInetAddressTable.MasterId.Value = masterId;
            coInetAddressTable.AddressCode.Value = addressCode;

            try
            {
                lastError = coInetAddressTable.Get();
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound)
                    {
                        throw new Exception(lastError.ToString());
                    }
                }
                emailAddress = coInetAddressTable.INet1.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                coInetAddressTable.Close();
            }
            return emailAddress;

        }

        private static Address GetCustomerAddress(string customerNumber, string addressCode)
        {
            TableError lastError;
            var CustomerMstrAddr = Dynamics.Tables.RmCustomerMstrAddr;
            Address address;

            try
            {
                // Set the key to use for the table
                CustomerMstrAddr.Key = 1;

                CustomerMstrAddr.CustomerNumber.Value = customerNumber;
                CustomerMstrAddr.AddressCode.Value = addressCode;

                lastError = CustomerMstrAddr.Get();
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound)
                    {
                        throw new Exception(lastError.ToString());
                    }
                }

                address = new Address()
                {
                    ContactPerson = CustomerMstrAddr.ContactPerson.Value,
                    Address1 = CustomerMstrAddr.Address1.Value,
                    Address2 = CustomerMstrAddr.Address2.Value,
                    Address3 = CustomerMstrAddr.Address3.Value,
                    City = CustomerMstrAddr.City.Value,
                    State = CustomerMstrAddr.State.Value,
                    ZipCode = CustomerMstrAddr.Zip.Value,
                    Country = CustomerMstrAddr.Country.Value,
                    CountryCode = CustomerMstrAddr.CountryCode.Value,
                    Fax = CustomerMstrAddr.Fax.Value,
                    Phone = CustomerMstrAddr.Phone1.Value,
                    Email = GetEmailAddress("CUS", customerNumber, addressCode)

                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                CustomerMstrAddr.Close();
            }
            return address;
        }

        public static Customer GetCustomer(string customerNumber)
        {
            TableError lastError;
            var customerMstrTable = Dynamics.Tables.RmCustomerMstr;
            Customer customer;

            try
            {
                // Set the key to use for the table
                customerMstrTable.Key = 1;

                customerMstrTable.CustomerNumber.Value = customerNumber;

                lastError = customerMstrTable.Get();
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound)
                    {
                        throw new Exception(lastError.ToString());
                    }
                }

                customer = new Customer()
                {
                    CustomerNumber = customerMstrTable.CustomerNumber.Value,
                    CustomerName = customerMstrTable.CustomerName.Value,
                    Comment1 = customerMstrTable.Comment1.Value,
                    Comment2 = customerMstrTable.Comment2.Value,
                    PrimaryShipToAddressCode = customerMstrTable.PrimaryShiptoAddressCode.Value,
                    CustomerPriority = customerMstrTable.CustomerPriority.Value
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                customerMstrTable.Close();
            }

                return customer;
            }

        private static CustomerItemXref GetCustomerItemXref(string itemNumber, string customerNumber)
        {
            TableError lastError;

            var CustomerItemXrefTable = Dynamics.Tables.SopCustomerItemXref;
            CustomerItemXref xref;
            try
            {
                // Set the key to use for the table
                CustomerItemXrefTable.Key = 1;

                CustomerItemXrefTable.CustomerNumber.Value = customerNumber;
                CustomerItemXrefTable.ItemNumber.Value = itemNumber;

                lastError = CustomerItemXrefTable.Get();
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound)
                    {
                        throw new Exception(lastError.ToString());
                    }
                }

                xref = new CustomerItemXref()
                {
                    CustomerItemDescription = CustomerItemXrefTable.CustomerItemDescription.Value,
                    CustomerItemGenericDescription = CustomerItemXrefTable.CustomerItemGenericDescription.Value,
                    CustomerItemNumber = CustomerItemXrefTable.CustomerItemNumber.Value,
                    CustomerItemShortName = CustomerItemXrefTable.CustomerItemShortName.Value,
                    CustomerNumber = CustomerItemXrefTable.CustomerNumber.Value,
                    ItemNumber = CustomerItemXrefTable.ItemNumber.Value,
                    NoteIndex = CustomerItemXrefTable.NoteIndex.Value,
                    UserDefined1 = CustomerItemXrefTable.UserDefined1.Value,
                    UserDefined2 = CustomerItemXrefTable.UserDefined2.Value,
                    UserDefined3 = CustomerItemXrefTable.UserDefined3.Value,
                    UserDefined4 = CustomerItemXrefTable.UserDefined4.Value,
                    UserDefined5 = CustomerItemXrefTable.UserDefined5.Value,
                    UserDefinedKey1 = CustomerItemXrefTable.UserDefinedKey1.Value,
                    UserDefinedKey2 = CustomerItemXrefTable.UserDefinedKey2.Value,
                    UserDefinedKey3 = CustomerItemXrefTable.UserDefinedKey3.Value,
                    UserDefinedKey4 = CustomerItemXrefTable.UserDefinedKey4.Value,

                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                CustomerItemXrefTable.Close();
            }

            return xref;
        }


        public static List<SalesOrderLineTransferRequest> GetSalesOrderRequests(short sopType, string sopNumber)
        {
            var salesOrderTransferRequests = new List<SalesOrderLineTransferRequest>();
            TableError lastError;
            var SoLneTransfer = BusinessSolutionPartners.Tables.BspPowerhouseSoLneTransfer;

            SoLneTransfer.Key = 1;
            // Start of the range
            SoLneTransfer.Clear();
            SoLneTransfer.SopType.Value = sopType;
            SoLneTransfer.SopNumber.Value = sopNumber;
            SoLneTransfer.RangeStart();

            // End of the range
            SoLneTransfer.Fill();
            SoLneTransfer.SopType.Value = sopType;
            SoLneTransfer.SopNumber.Value = sopNumber;
            SoLneTransfer.RangeEnd();

            // Read through all of the rows in the range
            lastError = SoLneTransfer.GetFirst();
            while (lastError == TableError.NoError)
            {
                salesOrderTransferRequests.Add(new SalesOrderLineTransferRequest
                {
                    SopNumber = SoLneTransfer.SopNumber.Value,
                    SopType = SoLneTransfer.SopType.Value,
                    RequestedShipDate = SoLneTransfer.RequestedShipDate.Value,
                    ComponentSequence = SoLneTransfer.ComponentSequence.Value,
                    LineItemSequence = SoLneTransfer.LineItemSequence.Value,
                    VersionNumber = SoLneTransfer.VersionNumber.Value,
                    Qty = SoLneTransfer.Qty.Value,
                    QtyAllocated = SoLneTransfer.QtyAllocated.Value,
                    QtyPrevInvoiced = SoLneTransfer.QtyPrevInvoiced.Value,
                    DateSent = SoLneTransfer.DateSent.Value,
                    TimeSent = SoLneTransfer.TimeSent.Value,
                    RequestedBy = SoLneTransfer.RequestedBy.Value,
                });
                lastError = SoLneTransfer.GetNext();
            }
            // Close the table
            SoLneTransfer.Close();
            return salesOrderTransferRequests;
        }

        private static SalesOrderLineTransferRequest GetSalesLineTransferRequest(short sopType, string sopNumber, int componentSeq, int lineItemSeq)
        {
            TableError lastError;
            var SoLneTransfer = BusinessSolutionPartners.Tables.BspPowerhouseSoLneTransfer;
            SalesOrderLineTransferRequest request;
            try
            {
                // Set the key to use for the table
                // 4 components - SOP Number, SOP Type, Component Sequence, Line Item Sequence
                SoLneTransfer.Key = 1;

                SoLneTransfer.SopType.Value = sopType;
                SoLneTransfer.SopNumber.Value = sopNumber;
                SoLneTransfer.LineItemSequence.Value = lineItemSeq;
                SoLneTransfer.ComponentSequence.Value = componentSeq;

                lastError = SoLneTransfer.Get();
                if (lastError != TableError.NoError)
                {
                    SoLneTransfer.Close();
                    return null;
                }

                request = new SalesOrderLineTransferRequest()
                {
                    SopNumber = SoLneTransfer.SopNumber.Value,
                    SopType = SoLneTransfer.SopType.Value,
                    RequestedShipDate = SoLneTransfer.RequestedShipDate.Value,
                    ComponentSequence = SoLneTransfer.ComponentSequence.Value,
                    LineItemSequence = SoLneTransfer.LineItemSequence.Value,
                    VersionNumber = SoLneTransfer.VersionNumber.Value,
                    Qty = SoLneTransfer.Qty.Value,
                    QtyAllocated = SoLneTransfer.QtyAllocated.Value,
                    QtyPrevInvoiced = SoLneTransfer.QtyPrevInvoiced.Value,
                    DateSent = SoLneTransfer.DateSent.Value,
                    TimeSent = SoLneTransfer.TimeSent.Value,
                    RequestedBy = SoLneTransfer.RequestedBy.Value,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                SoLneTransfer.Close();
            }
            return request;
        }

        private static RecordNotesMaster GetRecordNotes(decimal noteIndex)
        {
            TableError lastError;

            var RecordNotesMasterTable = Dynamics.Tables.SyRecordNotesMstr;
            RecordNotesMaster recordNote;

            try
            {
                // Set the key to use for the table
                RecordNotesMasterTable.Key = 1;

                RecordNotesMasterTable.NoteIndex.Value = noteIndex;

                lastError = RecordNotesMasterTable.Get();
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound)
                    {
                        throw new Exception(lastError.ToString());
                    }
                }

                recordNote = new RecordNotesMaster()
                {
                    Date = RecordNotesMasterTable.Date.Value,
                    Time = RecordNotesMasterTable.Time.Value,
                    NoteIndex = noteIndex,
                    TextField = RecordNotesMasterTable.TextField?.Value.SanitizeXMLString().RemoveSpecialCharacters()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                RecordNotesMasterTable.Close();
            }

            return recordNote;
        }

        private static CommentMaster GetCommentRecord(string commentID)
        {
            TableError lastError;

            var CommentMasterTable = Dynamics.Tables.SyCommentMstr;
            CommentMaster commentMaster;

            try
            {
                // Set the key to use for the table
                CommentMasterTable.Key = 1;

                CommentMasterTable.CommentId.Value = commentID;

                lastError = CommentMasterTable.Get();
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound && !Constants.SuppressUserErrorNotification)
                    {
                        throw new Exception(lastError.ToString());
                    }
                }

                commentMaster = new CommentMaster()
                {
                    CommentID = commentID,
                    NoteIndex = CommentMasterTable.NoteIndex.Value,
                    CommentText = CommentMasterTable.CommentText.Value,
                    Notes = GetRecordNotes(CommentMasterTable.NoteIndex.Value)
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                // Close the table
                CommentMasterTable.Close();
            }

            return commentMaster;
        }

        public static void UpdateSalesOrderRequests(PowerhouseWS.Order order)
        {
            TableError lastError;
            short sopType = 2;
            //var requestedShipDate = DateTime.ParseExact(order.releaseNum, "yyyyMMdd", CultureInfo.InvariantCulture);

            var SoLneTransfer = BusinessSolutionPartners.Tables.BspPowerhouseSoLneTransfer;


            foreach (var line in order.orderLines)
            {
                SoLneTransfer.Key = 1;

                SoLneTransfer.Clear();
                SoLneTransfer.SopType.Value = sopType;
                SoLneTransfer.SopNumber.Value = order.orderId;
                SoLneTransfer.ComponentSequence.Value = !string.IsNullOrWhiteSpace(line.hostId1) ? Convert.ToInt32(line.hostId1) : 0;
                SoLneTransfer.LineItemSequence.Value = !string.IsNullOrWhiteSpace(line.hostLine) ? Convert.ToInt32(line.hostLine) : 0;
                lastError = SoLneTransfer.Change();

                SoLneTransfer.ItemNumber.Value = line.itemId;
                SoLneTransfer.VersionNumber.Value = order.releaseNum;
                SoLneTransfer.RequestedShipDate.Value = order.shipByDate.Value;
                SoLneTransfer.Qty.Value = Convert.ToDecimal(line.piecesOrdered.Value);
                SoLneTransfer.QtyAllocated.Value = Convert.ToDecimal(line.piecesToPick.Value);
                SoLneTransfer.QtyPrevInvoiced.Value = !string.IsNullOrWhiteSpace(line.hostId2) ? Convert.ToDecimal(line.hostId2) : 0;
                SoLneTransfer.RequestedBy.Value = Dynamics.Globals.UserId;
                SoLneTransfer.TimeSent.Value = DateTime.Now;
                SoLneTransfer.DateSent.Value = DateTime.Now;
                lastError = SoLneTransfer.Save();

                SoLneTransfer.Release();
                // Close the table
                SoLneTransfer.Close();
            }


        }

        #endregion

        #region Receiving

        public static ReceiptTransfer GetReceiving(string containerID)
        {
            TableError lastError;
            TableError lastReceiptHeaderError;
            TableError lastLineError;
            ReceiptTransfer receiptTransfer;
            var receivingHdrTable = BusinessSolutionPartners.Tables.BspPowerhouseReceivingHdr;
            var receivingTransferTable = BusinessSolutionPartners.Tables.BspPowerhouseReceivingLine;
            var receivingTransferLineTable = Dynamics.Tables.PopReceiptLineHist;
            var receiptHistTable = Dynamics.Tables.PopReceiptHist;
            decimal qtyInvoiced, qtyShipped;
            int lineItemSequence = 16384;
            DateTime promisedDate;

            try
            {
                receivingHdrTable.Key = 1;

                receivingHdrTable.BspContainerId.Value = containerID;
                lastError = receivingHdrTable.Get();

                if (lastError != TableError.NoError)
                {
                    throw new Exception("Container record does not exist");
                }

                receiptTransfer = new ReceiptTransfer
                {
                    ContainerID = containerID,
                    CreatedDate = receivingHdrTable.DateSent.Value,                    
                    //ReceiptNumber = receivingTransferTable.PopReceiptNumber.Value,
                    //VendorID = receivingTransferTable.VendorId.Value,
                    //VendorName = receivingTransferTable.VendorName.Value,
                    //DocumentNumber = receivingTransferTable.VendorDocumentNumber.Value,
                };

                //Retrieve the Receiving Records
                // Specify the range for the table
                // Start of the range
                receivingTransferTable = BusinessSolutionPartners.Tables.BspPowerhouseReceivingLine;
                receivingTransferTable.Key = 1;
                receivingTransferTable.Clear();
                receivingTransferTable.BspContainerId.Value = containerID;
                receivingTransferTable.RangeStart();

                // End of the range
                receivingTransferTable.PopReceiptNumber.Fill();
                receivingTransferTable.BspContainerId.Value = containerID;
                receivingTransferTable.RangeEnd();

                // Read through all of the rows in the range
                lastError = receivingTransferTable.GetFirst();
                while (lastError == TableError.NoError)
                {
                    //Add code here to retrieve receipt header information
                    receiptHistTable.Release();
                    receiptHistTable.RangeClear();
                    receiptHistTable.Clear();

                    receiptHistTable.PopReceiptNumber.Value = receivingTransferTable.PopReceiptNumber.Value;
                    lastReceiptHeaderError = receiptHistTable.Get();
                    if (lastReceiptHeaderError == TableError.NoError)
                    {
                        // Added as per Margie's request 09122020
                        receiptTransfer.ActualShip = receiptHistTable.ActualShipDate.Value;
                    }

                    //Retrieve all Receipt Line Items
                    // Specify the range for the table
                    // Start of the range
                    receivingTransferLineTable.Release();
                    receivingTransferLineTable.Clear();
                    receivingTransferLineTable.RangeClear();

                    receivingTransferLineTable.Key = 1;

                    receivingTransferLineTable.PopReceiptNumber.Value = receivingTransferTable.PopReceiptNumber.Value;
                    receivingTransferLineTable.RangeStart();

                    // End of the range
                    receivingTransferLineTable.ReceiptLineNumber.Fill();
                    receivingTransferLineTable.PopReceiptNumber.Value = receivingTransferTable.PopReceiptNumber.Value;
                    receivingTransferLineTable.RangeEnd();

                    // Read through all of the rows in the range
                    lastLineError = receivingTransferLineTable.GetFirst();
                    while (lastLineError == TableError.NoError)
                    {
                        var itemMaster = GetItemMaster(receivingTransferLineTable.ItemNumber.Value);
                        string mappedLocation = GetLocationMapping(receivingTransferLineTable.LocationCode.Value);

                        // Check if mapped location is unique for this receipt
                        if (string.IsNullOrEmpty(receiptTransfer.LocationCode))
                            receiptTransfer.LocationCode = mappedLocation;
                        else if (receiptTransfer.LocationCode != mappedLocation)
                            throw new Exception("Mapped Location is not unique for this receipt. Please check the receipts and then try again.");

                        GetPODetails(receivingTransferLineTable.PopReceiptNumber.Value, receivingTransferLineTable.ReceiptLineNumber.Value, receivingTransferLineTable.PoNumber.Value, 
                            receivingTransferLineTable.ItemNumber.Value, out qtyInvoiced, out qtyShipped, out promisedDate);


                        receiptTransfer.Items.Add(new ReceiptTransferLine
                        {
                            ReceiptNumber = receivingTransferLineTable.PopReceiptNumber.Value,
                            LineItemSequence = lineItemSequence,
                            ReceiptLineItem = receivingTransferLineTable.ReceiptLineNumber.Value,
                            PoNumber = receivingTransferLineTable.PoNumber.Value,
                            ItemNumber = receivingTransferLineTable.ItemNumber.Value,
                            ItemDesc = receivingTransferLineTable.ItemDescription.Value,
                            VendorItemNumber = receivingTransferLineTable.VendorItemNumber.Value,
                            VendorItemDescription = receivingTransferLineTable.VendorItemDescription.Value,
                            UofMQtyInBase = receivingTransferLineTable.UOfMQtyInBase.Value,
                            ActualShipDate = receivingTransferLineTable.ActualShipDate.Value,
                            CommentID = receivingTransferLineTable.CommentId.Value,
                            UofM = receivingTransferLineTable.UOfM.Value,
                            UnitCost = receivingTransferLineTable.UnitCost.Value,
                            ExtendedCost = receivingTransferLineTable.ExtendedCost.Value,
                            LocationCode = receivingTransferLineTable.LocationCode.Value,
                            JobNumber = receivingTransferLineTable.JobNumber.Value,
                            TaxAmount = receivingTransferLineTable.TaxAmount.Value,
                            BackoutTaxAmount = receivingTransferLineTable.BackoutTaxAmount.Value,
                            InventoryAccountIndex = receivingTransferLineTable.InventoryIndex.Value,
                            InventoryAccount = GetGlAccountNumber(receivingTransferLineTable.InventoryIndex.Value),
                            ShipMethod = receivingTransferLineTable.ShippingMethod.Value,
                            QTYInvoiced = qtyInvoiced,
                            QTYShipped = qtyShipped,
                            PromisedDate = promisedDate,
                            ItemType = Convert.ToInt16(itemMaster?.ItemType),
                            NonIv = receivingTransferLineTable.NonIv.Value,
                            MappedLocationCode = mappedLocation
                        });

                        lineItemSequence += 16384;

                        lastLineError = receivingTransferLineTable.GetNext();
                    }
                    // Get the next line item
                    lastError = receivingTransferTable.GetNext();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                receivingHdrTable.Close();
                receivingTransferTable.Close();
                receivingTransferLineTable.Close();
            }

            return receiptTransfer;
        }

        private static void GetPODetails(string receiptNumber, int receiptLineNumber, string poNumber, string itemNumber, out decimal qtyInvoiced, out decimal qtyShipped, out DateTime promisedDate)
        {
            TableError lastError;
            TableError lastLineError = TableError.Unknown;

            decimal lqtyInvoiced = 0, lqtyShipped = 0;
            DateTime lpromisedDate = DateTime.Now;

            var popPoRcptApplyTable = Dynamics.Tables.PopPoRcptApply;
            var popPoLineTable = Dynamics.Tables.PopPoLine;


            try
            {
                if (!string.IsNullOrWhiteSpace(poNumber))
                {
                    //Retrieve the PO Details First
                    popPoLineTable.Key = 2;

                    popPoLineTable.ItemNumber.Value = itemNumber;
                    popPoLineTable.PoNumber.Value = poNumber;
                    popPoLineTable.RangeStart();
                    popPoLineTable.Ord.Fill();
                    popPoLineTable.ItemNumber.Value = itemNumber;
                    popPoLineTable.PoNumber.Value = poNumber;
                    popPoLineTable.RangeEnd();

                    // Read through all of the rows in the range
                    lastError = popPoLineTable.GetFirst();
                    while (lastError == TableError.NoError)
                    {
                        popPoRcptApplyTable.Key = 1;
                        
                        popPoRcptApplyTable.PopReceiptNumber.Value = receiptNumber;
                        popPoRcptApplyTable.ReceiptLineNumber.Value = receiptLineNumber;
                        popPoRcptApplyTable.PoNumber.Value = poNumber;
                        popPoRcptApplyTable.PoLineNumber.Value = popPoLineTable.Ord.Value;
                        lastLineError = popPoRcptApplyTable.Get();
                        if (lastLineError == TableError.NoError)
                        {
                            lqtyInvoiced += popPoRcptApplyTable.QtyInvoiced.Value;
                            lqtyShipped += popPoRcptApplyTable.QtyShipped.Value;
                        }
                        lpromisedDate = popPoLineTable.PromisedDate.Value;

                        lastError = popPoLineTable.GetNext();
                    }
                }
                else
                {
                    popPoRcptApplyTable.Key = 1;
                    popPoRcptApplyTable.PopReceiptNumber.Value = receiptNumber;
                    popPoRcptApplyTable.ReceiptLineNumber.Value = receiptLineNumber;
                    popPoRcptApplyTable.RangeStart();
                    popPoRcptApplyTable.PoNumber.Fill();
                    popPoRcptApplyTable.PoLineNumber.Fill();
                    popPoRcptApplyTable.RangeEnd();
                    lastLineError = popPoRcptApplyTable.GetFirst();

                    if (lastLineError == TableError.NoError)
                    {
                        lqtyInvoiced += popPoRcptApplyTable.QtyInvoiced.Value;
                        lqtyShipped += popPoRcptApplyTable.QtyShipped.Value;
                    }
                    lpromisedDate = popPoLineTable.PromisedDate.Value;
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                popPoRcptApplyTable.Close();
                popPoLineTable.Close();
            }

            qtyInvoiced = lqtyInvoiced;
            qtyShipped = lqtyShipped;
            promisedDate = lpromisedDate;
        }

        public static void UpdatePORequests(PowerhouseWS.Receipt receipt)
        {
            TableError lastError;
            //var requestedShipDate = DateTime.ParseExact(order.releaseNum, "yyyyMMdd", CultureInfo.InvariantCulture);

            var receivingTransfer = BusinessSolutionPartners.Tables.BspPowerhouseReceivingTransfer;

            
            foreach (var line in receipt.receiptLines)
            {
                try
                {
                    receivingTransfer.Key = 1;

                    receivingTransfer.Clear();
                    receivingTransfer.BspContainerId.Value = receipt.po;
                    receivingTransfer.PopReceiptNumber.Value = line.rlCust1;
                    receivingTransfer.PoNumber.Value = line.rlCust3;
                    receivingTransfer.ItemNumber.Value = line.itemId;
                    lastError = receivingTransfer.Change();

                    receivingTransfer.ItemDescription.Value = line.rlCust4;
                    receivingTransfer.VendorItemNumber.Value = line.rlCust5;
                    receivingTransfer.VendorItemDescription.Value = line.rlCust6;
                    receivingTransfer.UOfMQtyInBase.Value = Convert.ToDecimal(line.rlCust10);
                    receivingTransfer.ActualShipDate.Value = Convert.ToDateTime(line.rlCust7);
                    receivingTransfer.PromisedDate.Value = Convert.ToDateTime(line.dateExpected);
                    receivingTransfer.QtyShipped.Value = Convert.ToDecimal(line.piecesOrdered.Value);
                    receivingTransfer.UOfM.Value = line.usrUom;
                    receivingTransfer.LocationCode.Value = line.rlCust2;
                    receivingTransfer.ShippingMethod.Value = line.rlCust8;
                    receivingTransfer.RequestedBy.Value = Dynamics.Globals.UserId;
                    receivingTransfer.TimeSent.Value = DateTime.Now;
                    receivingTransfer.DateSent.Value = DateTime.Now;
                    lastError = receivingTransfer.Save();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    receivingTransfer.Release();
                    receivingTransfer.Close();
                }
            }
        }

        public static void DeletePORequests(PowerhouseWS.Receipt receipt)
        {
            TableError lastError;

            var bspPowerhouseReceivingHdrTable = BusinessSolutionPartners.Tables.BspPowerhouseReceivingHdr;
            var bspPowerhouseReceivingLineTable = BusinessSolutionPartners.Tables.BspPowerhouseReceivingLine;
            var bspPowerhouseReceivingTransferTable = BusinessSolutionPartners.Tables.BspPowerhouseReceivingTransfer;

            try
            {
                bspPowerhouseReceivingHdrTable.Key = 1;

                bspPowerhouseReceivingHdrTable.BspContainerId.Value = receipt.po;
                lastError = bspPowerhouseReceivingHdrTable.Change();
                if (lastError == TableError.NoError)
                    if (bspPowerhouseReceivingHdrTable.Remove() != TableError.NoError)
                        throw new Exception("Error deleting record in Receiving Header.");

                bspPowerhouseReceivingLineTable.Key = 1;

                bspPowerhouseReceivingLineTable.BspContainerId.Value = receipt.po;
                bspPowerhouseReceivingLineTable.RangeStart();
                bspPowerhouseReceivingLineTable.PopReceiptNumber.Fill();
                bspPowerhouseReceivingLineTable.RangeEnd();

                if (bspPowerhouseReceivingLineTable.RangeRemove() != TableError.NoError)
                    throw new Exception("Error deleting records in Receiving Line.");

                bspPowerhouseReceivingTransferTable.Key = 1;

                bspPowerhouseReceivingTransferTable.BspContainerId.Value = receipt.po;
                bspPowerhouseReceivingTransferTable.RangeStart();
                bspPowerhouseReceivingTransferTable.PopReceiptNumber.Fill();
                bspPowerhouseReceivingTransferTable.PoNumber.Fill();
                bspPowerhouseReceivingTransferTable.ItemNumber.Fill();
                bspPowerhouseReceivingTransferTable.RangeEnd();

                if (bspPowerhouseReceivingTransferTable.RangeRemove() != TableError.NoError)
                    throw new Exception("Error deleting records in Receiving Transfer.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                bspPowerhouseReceivingHdrTable.Close();
                bspPowerhouseReceivingLineTable.Close();
                bspPowerhouseReceivingTransferTable.Close();
            }

        }

        #endregion

        #region Purchase Order

        public static PurchaseOrder GetPurchaseOrder(string poNumber)
        {
            TableError lastError;

            var PopPoTable = Dynamics.Tables.PopPo;
            var PopPoLineTable = Dynamics.Tables.PopPoLine;
            PurchaseOrder purchaseOrder;
            try
            {
                PopPoTable.Key = 1;
                PopPoTable.PoNumber.Value = poNumber;
                lastError = PopPoTable.Get();
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound)
                    {
                        throw new Exception(lastError.ToString());
                    }
                }

                purchaseOrder = new PurchaseOrder
                {
                    PoNumber = PopPoTable.PoNumber.Value,
                    PoType = PopPoTable.PoType.Value,
                    VendorId = PopPoTable.VendorId.Value,
                    VendorName = PopPoTable.VendorName.Value,
                    BuyerId = PopPoTable.BuyerId.Value,
                    DocumentDate = PopPoTable.DocumentDate.Value,
                    PromisedDate = PopPoTable.PromisedDate.Value,
                    PromisedShipDate = PopPoTable.PromisedShipDate.Value,
                    ShippingMethod = PopPoTable.ShippingMethod.Value,
                    CreatedDate = PopPoTable.CreatedDate.Value,
                    LastEditDate = PopPoTable.LastEditDate.Value,
                    LastPrintedDate = PopPoTable.LastPrintedDate.Value,
                    ModifiedDate = PopPoTable.ModifiedDate.Value,
                    UserToEnter = PopPoTable.UserToEnter.Value,
                    PoStatus = PopPoTable.PoStatus.Value,
                    CustomerNumber = PopPoTable.CustomerNumber.Value,
                    RequiredDate = PopPoTable.RequiredDate.Value,
                    RequisitionDate = PopPoTable.RequisitionDate.Value,
                    RevisionNumber = PopPoTable.RevisionNumber.Value,
                    PurchaseCompanyName = PopPoTable.PurchaseCompanyName.Value,
                    PurchaseContact = PopPoTable.PurchaseContact.Value,
                    PurchaseAddress1 = PopPoTable.PurchaseAddress1.Value,
                    PurchaseAddress2 = PopPoTable.PurchaseAddress2.Value,
                    PurchaseAddress3 = PopPoTable.PurchaseAddress3.Value,
                    PurchaseCity = PopPoTable.PurchaseCity.Value,
                    PurchaseState = PopPoTable.PurchaseState.Value,
                    PurchaseZipCode = PopPoTable.PurchaseZipCode.Value,
                    PurchaseCountryCode = PopPoTable.PurchaseCountryCode.Value,
                    PurchaseCountry = PopPoTable.PurchaseCountry.Value,
                    PurchasePhone1 = PopPoTable.PurchasePhone1.Value,
                    PurchasePhone2 = PopPoTable.PurchasePhone2.Value,
                    PurchasePhone3 = PopPoTable.PurchasePhone3.Value,
                    PurchaseFax = PopPoTable.PurchaseFax.Value,
                    CommentId = PopPoTable.CommentId.Value,
                    ChangeOrderFlag = PopPoTable.ChangeOrderFlag.Value,

                };
                // Specify the range for the table
                // Start of the range
                
                PopPoLineTable.Clear();
                PopPoLineTable.PoNumber.Value = poNumber;
                PopPoLineTable.RangeStart();

                // End of the range
                PopPoLineTable.Fill();
                PopPoLineTable.PoNumber.Value = poNumber;
                PopPoLineTable.RangeEnd();

                // Read through all of the rows in the range
                lastError = PopPoLineTable.GetFirst();
                while (lastError == TableError.NoError)
                {
                    string mappedLocation = GetLocationMapping(PopPoLineTable.LocationCode.Value);
                    purchaseOrder.LocationCode = mappedLocation;

                    var itemMaster = GetItemMaster(PopPoLineTable.ItemNumber.Value);
                    purchaseOrder.Items.Add(new PurchaseOrderLine
                    {
                        PoNumber = PopPoLineTable.PoNumber.Value,
                        ItemNumber = PopPoLineTable.ItemNumber.Value,
                        CommentId = PopPoLineTable.CommentId.Value,
                        DocumentType = PopPoLineTable.DocumentType.Value,
                        InventoryIndex = PopPoLineTable.InventoryIndex.Value,
                        InventoryAccountNumber = GetGlAccountNumber(PopPoLineTable.InventoryIndex.Value),
                        ItemDescription = PopPoLineTable.ItemDescription.Value,
                        ItemTrackingOption = PopPoLineTable.ItemTrackingOption.Value,
                        LineNumber = PopPoLineTable.LineNumber.Value,
                        Ord = PopPoLineTable.Ord.Value,
                        PoLineStatus = PopPoLineTable.PoLineStatus.Value,
                        PoType = PopPoLineTable.PoType.Value,
                        PromisedDate = PopPoLineTable.PromisedDate.Value,
                        PromisedShipDate = PopPoLineTable.PromisedShipDate.Value,
                        QtyCanceled = PopPoLineTable.QtyCanceled.Value,
                        QtyCommittedInBase = PopPoLineTable.QtyCommittedInBase.Value,
                        QtyOrdered = PopPoLineTable.QtyOrdered.Value,
                        QtyUncommittedInBase = PopPoLineTable.QtyUncommittedInBase.Value,
                        ReleaseByDate = PopPoLineTable.ReleaseByDate.Value,
                        RequiredDate = PopPoLineTable.RequiredDate.Value,
                        RequestedBy = PopPoLineTable.RequestedBy.Value,
                        UOfMQtyInBase = PopPoLineTable.UOfMQtyInBase.Value,
                        UOfM = PopPoLineTable.UOfM.Value,
                        VendorId = PopPoLineTable.VendorId.Value,
                        VendorItemDescription = PopPoLineTable.VendorItemDescription.Value,
                        VendorItemNumber = PopPoLineTable.VendorItemNumber.Value,
                        LocationCode = mappedLocation,
                        ItemType = itemMaster != null ? itemMaster.ItemType : 0,
                        NonIv = PopPoLineTable.NonIv.Value,
                        QtyRemainingToShip = GetQuantityRemainingToShip(PopPoLineTable.PoNumber.Value, PopPoLineTable.Ord.Value, PopPoLineTable.QtyOrdered.Value, PopPoLineTable.QtyCanceled.Value)

                    });
                    // Get the next line item
                    lastError = PopPoLineTable.GetNext();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                PopPoTable.Close();
                PopPoLineTable.Close();
            }
            return purchaseOrder;
        }

        private static decimal GetQuantityRemainingToShip(string poNumber, int poLineNumber, decimal qtyOrdered, decimal qtyCanceled)
        {
            decimal remainingToShip = 0;
            decimal shipped = 0;
            decimal rejected = 0;
            decimal replaced = 0;

            TableError lastError;
            var PoRcptApplyTable = Dynamics.Tables.PopPoRcptApply;

            PoRcptApplyTable.Key = 2;

            PoRcptApplyTable.Clear();
            PoRcptApplyTable.PoNumber.Value = poNumber;
            PoRcptApplyTable.PoLineNumber.Value = poLineNumber;
            PoRcptApplyTable.RangeStart();


            // End of the range
            PoRcptApplyTable.Fill();
            PoRcptApplyTable.PoNumber.Value = poNumber;
            PoRcptApplyTable.PoLineNumber.Value = poLineNumber;
            PoRcptApplyTable.RangeEnd();

            // Read through all of the rows in the range
            lastError = PoRcptApplyTable.GetFirst();
            while (lastError == TableError.NoError)
            {
                if (PoRcptApplyTable.Status != 2)
                {
                    //Receipt not voided
                    shipped += PoRcptApplyTable.QtyShipped.Value;
                    rejected += PoRcptApplyTable.QtyRejected.Value;
                    replaced += PoRcptApplyTable.QtyReplaced.Value;
                }
                lastError = PoRcptApplyTable.GetNext();
            }
            // Close the table
            PoRcptApplyTable.Close();

            remainingToShip = qtyOrdered - qtyCanceled - (shipped - rejected - replaced);
            if (remainingToShip < 0)
                remainingToShip = 0;

            return remainingToShip;

        }

        #endregion

        #region Utils

        public static string GetLocationMapping(string siteId)
        {
            TableError lastError;
            string location;
            var locationMappingTable = BusinessSolutionPartners.Tables.BspPowerhouseLocationMapping;

            try
            {
                locationMappingTable.Key = 1;

                locationMappingTable.Clear();
                locationMappingTable.LocationCode.Value = siteId;
                locationMappingTable.BspPhWarehouseId.Value = string.Empty;
                locationMappingTable.RangeStart();

                // End of the range
                locationMappingTable.Fill();
                locationMappingTable.LocationCode.Value = siteId;
                locationMappingTable.RangeEnd();

                // Read through all of the rows in the range
                lastError = locationMappingTable.GetFirst();
                if (lastError != TableError.NoError)
                {
                    throw new Exception("Location is not mapped.");
                }

                //if code reached here, it means record was found
                location = locationMappingTable.BspPhWarehouseId.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                locationMappingTable.Close();
            }

            return location;
        }

        private static string GetGlAccountNumber(int acctIndx)
        {
            TableError lastError;
            string accountNumber;

            var glAccountIndexMstr = Dynamics.Tables.GlAccountIndexMstr;
            try
            {
                glAccountIndexMstr.Key = 1;

                glAccountIndexMstr.AccountIndex.Value = acctIndx;

                lastError = glAccountIndexMstr.Get();
                if (lastError != TableError.NoError)
                {
                    if (lastError != TableError.NotFound && !Constants.SuppressUserErrorNotification)
                    {
                        throw new Exception(lastError.ToString());
                    }
                }

                accountNumber = glAccountIndexMstr.AccountNumberString.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                glAccountIndexMstr.Close();
            }
            return accountNumber;
        }

        private static string TrimAndRemoveCRLFFromString(string value)
        {
            string result = TrimIt(value);

            if (!string.IsNullOrWhiteSpace(result))
            {
                //Remove carriage returns
                result = result.Replace("\n", "").Replace("\r", "");
            }
            return result;
        }

        private static string TrimIt(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();

            return value;
        }

        public static void LogReceiptTransfer(string containerID, string statusCode = "", string errMessage = "")
        {
            TableError lastError;

            var bspPowerhouseReceivingHdrTable = BusinessSolutionPartners.Tables.BspPowerhouseReceivingHdr;
            try
            {
                bspPowerhouseReceivingHdrTable.Release();
                bspPowerhouseReceivingHdrTable.Clear();
                bspPowerhouseReceivingHdrTable.RangeClear();

                bspPowerhouseReceivingHdrTable.Key = 1;
                bspPowerhouseReceivingHdrTable.BspContainerId.Value = containerID;

                bspPowerhouseReceivingHdrTable.Change();

                bspPowerhouseReceivingHdrTable.ErrorMessageText.Value = errMessage ?? string.Empty;

                bspPowerhouseReceivingHdrTable.RequestedBy.Value = Dynamics.Globals.UserId;
                bspPowerhouseReceivingHdrTable.TimeSent.Value = DateTime.Now;
                bspPowerhouseReceivingHdrTable.DateSent.Value = DateTime.Now;
                bspPowerhouseReceivingHdrTable.StatusCode.Value = statusCode;

                lastError = bspPowerhouseReceivingHdrTable.Save();
                if (lastError != TableError.NoError)
                    throw new Exception(lastError.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                bspPowerhouseReceivingHdrTable.Close();
            }
        }
        public static void LogSoTransfer(short sopType, string sopNumber, string versionNumber = "", string statusCode = "", string errMessage = "")
        {
            var PwsSoLogTable = BusinessSolutionPartners.Tables.BspPowerhouseSoLog;

            try
            {
                PwsSoLogTable.SopType.Value = sopType;
                PwsSoLogTable.SopNumber.Value = sopNumber;
                PwsSoLogTable.VersionNumber.Value = versionNumber ?? string.Empty;
                PwsSoLogTable.Change();
                PwsSoLogTable.StatusCode.Value = statusCode ?? string.Empty;
                PwsSoLogTable.ErrorMessageText.Value = errMessage ?? string.Empty;
                PwsSoLogTable.RequestedBy.Value = Dynamics.Globals.UserId;
                PwsSoLogTable.TimeSent.Value = DateTime.Now;
                PwsSoLogTable.DateSent.Value = DateTime.Now;

                PwsSoLogTable.Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                PwsSoLogTable.Close();
            }
        }

        public static void LogItemTransfer(string itemNumber, string statusCode = "", string errMessage = "")
        {
            var ItemTransferTable = BusinessSolutionPartners.Tables.BspPowerhouseItemTransfer;

            try
            {
                ItemTransferTable.ItemNumber.Value = itemNumber;
                ItemTransferTable.Change();
                ItemTransferTable.StatusCode.Value = statusCode;
                ItemTransferTable.ErrorMessageText.Value = errMessage ?? string.Empty;
                ItemTransferTable.RequestedBy.Value = Dynamics.Globals.UserId;
                ItemTransferTable.TimeSent.Value = DateTime.Now;
                ItemTransferTable.DateSent.Value = DateTime.Now;

                ItemTransferTable.Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                ItemTransferTable.Close();
            }
        }

        public static void LogPOTransfer(string poNumber, string statusCode = "", string errMessage = "")
        {
            var PoTransferTable = BusinessSolutionPartners.Tables.BspPowerhousePoTransfer;

            try
            {
                PoTransferTable.PoNumber.Value = poNumber;
                PoTransferTable.Change();
                 PoTransferTable.StatusCode.Value = statusCode;
                PoTransferTable.ErrorMessageText.Value = errMessage ?? string.Empty;
                PoTransferTable.RequestedBy.Value = Dynamics.Globals.UserId;
                PoTransferTable.TimeSent.Value = DateTime.Now;
                PoTransferTable.DateSent.Value = DateTime.Now;

                PoTransferTable.Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Close the table
                PoTransferTable.Close();
            }
        }

        #endregion



    }
}
