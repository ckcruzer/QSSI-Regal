﻿using System;
using System.Collections.Generic;
using System.Linq;
using BSP.PowerHouse.DynamicsGP.Integration.PowerHouseWS;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using BSP.PowerHouse.DynamicsGP.Integration.Extensions;
using BSP.PowerHouse.DynamicsGP.Integration.Data;
using BSP.PowerHouse.DynamicsGP.Integration.Domain;
using BSP.PowerHouse.DynamicsGP.Integration.Model;

namespace BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects
{
    public class SopTransaction : IEConnectObject
    {
        private readonly Shipment _shipment;
        private readonly PowerhouseWsSetting _powerhouseWsSetting;
        public SopTransaction(Shipment shipment, PowerhouseWsSetting powerhouseWsSetting)
        {
            this._shipment = shipment;
            this._powerhouseWsSetting = powerhouseWsSetting;
        }

        public string GetXmlSerializedObject()
        {
            if (_shipment == null)
                return null;

            // Instantiate an eConnectType schema object
            eConnectType eConnect = new eConnectType();

            // Create a eConnect SOPTransactionType schema object
            SOPTransactionType salesOrder = new SOPTransactionType();

            // Insert a Line to fulfill

            // Populate the schema object
            salesOrder.taSopHdrIvcInsert = GetHeaderRecord();
            salesOrder.taSopLineIvcInsert_Items = GetLineItems();
            salesOrder.taSopLineIvcInsertComponent_Items = GetLineKitItems();
            salesOrder.taSopTrackingNum_Items = GetTrackingInfo();

            // Create an array that holds SOPTransactionType objects
            // Populate the array with the SOPTransactionType schema object
            SOPTransactionType[] sopTransactionType = { salesOrder };

            eConnect.SOPTransactionType = sopTransactionType;

            return eConnect.SerializeObject();
        }
        public taSopTrackingNum_ItemsTaSopTrackingNum[] GetTrackingInfo()
        {
            var items = new List<taSopTrackingNum_ItemsTaSopTrackingNum>();
            foreach (var carton in _shipment.shipmentCartons)
            {
                if (!string.IsNullOrWhiteSpace(carton.packageTraceId))
                {
                    //check if already added. this to count for multiple cartoons with the same tracking number
                    if(!items.Any(x => x.Tracking_Number.Equals(carton.packageTraceId, StringComparison.OrdinalIgnoreCase)))
                    {
                        items.Add(new taSopTrackingNum_ItemsTaSopTrackingNum
                        {
                            SOPTYPE = (short)GpSopType.Order,
                            SOPNUMBE = carton.orderId,
                            Tracking_Number = carton.packageTraceId,
                        });
                    }
                }
            }

            return items.ToArray();
        }

        public taSopHdrIvcInsert GetHeaderRecord()
        {
            SOP10100_DTO sopHeader = DynamicsGpDB.GetSalesTransactionHeader((short)GpSopType.Order, _shipment.orderId);

            if (sopHeader == null)
                throw new Exception("Record does not exist. " + _shipment.orderId);

            
            var header = new taSopHdrIvcInsert()
            {
                SOPTYPE = (short)GpSopType.Order,
                SOPNUMBE = _shipment.orderId,
                DOCID = sopHeader.DOCID,
                CUSTNMBR = sopHeader.CUSTNMBR,
                TAXSCHID = sopHeader.TAXSCHID,
                FRTSCHID = sopHeader.FRTSCHID,
                MSCSCHID = sopHeader.MSCSCHID,
                SHIPMTHD = _shipment.shipMethod,
                LOCNCODE = sopHeader.LOCNCODE,
                DOCDATE = sopHeader.DOCDATE.GpFormattedDate(),
                FREIGHT = _shipment.frtCharge.HasValue ? Convert.ToDecimal(_shipment?.frtCharge) : sopHeader.FRTAMNT,
                MISCAMNT = sopHeader.MISCAMNT,
                //TRDISAMT = sopHeader.TRDISAMT,
                //TRDISAMTSpecified = sopHeader.TRDISAMT > 0 ? true : false,
                //TRADEPCT = sopHeader.TRDISPCT,
                //TRADEPCTSpecified = sopHeader.TRDISPCT > 0 ? true : false,
                DISTKNAM = sopHeader.DISTKNAM,
                CSTPONBR = sopHeader.CSTPONBR,
                ShipToName = sopHeader.ShipToName,
                ADDRESS1 = sopHeader.ADDRESS1,
                ADDRESS2 = sopHeader.ADDRESS2,
                ADDRESS3 = sopHeader.ADDRESS3,
                CNTCPRSN = sopHeader.CNTCPRSN,
                FAXNUMBR = sopHeader.FAXNUMBR,
                CITY = sopHeader.CITY,
                STATE = sopHeader.STATE,
                ZIPCODE = sopHeader.ZIPCODE,
                COUNTRY = sopHeader.COUNTRY,
                PHNUMBR1 = sopHeader.PHNUMBR1,
                PHNUMBR2 = sopHeader.PHNUMBR2,
                PHNUMBR3 = sopHeader.PHONE3,
                PYMTRCVD = sopHeader.PYMTRCVD,
                SALSTERR = sopHeader.SALSTERR,
                SLPRSNID = sopHeader.SLPRSNID,
                UPSZONE = sopHeader.UPSZONE,
                BACHNUMB = _powerhouseWsSetting.BSPSOPFulfillmentBatchID, // Transfer to new batch
                PRBTADCD = sopHeader.PRBTADCD,
                PRSTADCD = sopHeader.PRSTADCD,
                PYMTRMID = sopHeader.PYMTRMID,
                DUEDATE = sopHeader.DUEDATE.GpFormattedDate(),
                DISCDATE = sopHeader.DISCDATE.GpFormattedDate(),
                COMMAMNT = sopHeader.COMMAMNT,
                DISAVAMT = sopHeader.DISAVAMT,
                DSCDLRAM = sopHeader.DSCDLRAM,
                DSCPCTAM = sopHeader.DSCPCTAM,
                COMMNTID = sopHeader.COMMNTID,
                ReqShipDate = sopHeader.ReqShipDate.GpFormattedDate(),
                UpdateExisting = 1,
                QUOEXPDA = sopHeader.QUOEXPDA.GpFormattedDate(),
                QUOTEDAT = sopHeader.QUOTEDAT.GpFormattedDate(),
                ORDRDATE = sopHeader.ORDRDATE.GpFormattedDate(),
                INVODATE = sopHeader.INVODATE.GpFormattedDate(),
                BACKDATE = sopHeader.BACKDATE.GpFormattedDate(),
                RETUDATE = sopHeader.RETUDATE.GpFormattedDate(),
                PRCLEVEL = sopHeader.PRCLEVEL,
                TAXEXMT1 = sopHeader.TAXEXMT1,
                TAXEXMT2 = sopHeader.TAXEXMT2,
                TXRGNNUM = sopHeader.TXRGNNUM,
                //REPTING = sopHeader.REPTING,
                //TRXFREQU = sopHeader.TRXFREQU,
                //TIMETREP = sopHeader.TIMETREP,
                CURNCYID = Configuration.AppSettings.DefaultCurrency,
                //DEFTAXSCHDS = 1
            };

            if (sopHeader.TRDISPCT > 0)
            {
                header.TRADEPCT = sopHeader.TRDISPCT / 100;
                header.TRADEPCTSpecified = true;
            }
            else if(sopHeader.TRDISAMT > 0)
            {
                header.TRDISAMT = sopHeader.TRDISAMT;
                header.TRDISAMTSpecified = true;
            }
            

            return header;
        }

        public taSopLineIvcInsert_ItemsTaSopLineIvcInsert[] GetLineItems()
        {
            var items = new List<taSopLineIvcInsert_ItemsTaSopLineIvcInsert>();

            foreach (var carton in _shipment.shipmentCartons)
            {
                foreach (var detail in carton.shipmentCartonDetail)
                {
                    //Get the Line Item Sequence First
                    if (!detail.orderLine.HasValue)
                        throw new Exception($"order line id is missing. ");

                    //var lnitmseq = Convert.ToInt32(Math.Truncate(Convert.ToDecimal(detail.orderLine.Value / 16384)) * 16384);
                    //var cmpntseq = (detail.orderLine.Value - lnitmseq) * 16384;

                    var lnitmseq = Convert.ToInt32(detail.olCust29);
                    var cmpntseq = Convert.ToInt32(detail.olCust30);

                    if (cmpntseq > 0)
                        continue;
                    
                    SOP10200_DTO sopLineItem = DynamicsGpDB.GetSalesTransactionLine((short)GpSopType.Order, detail.orderId, cmpntseq, lnitmseq);

                    if (sopLineItem == null)
                        throw new Exception($"Item {detail.itemId} on sales order {detail.orderId} line id {lnitmseq} is missing. ");

                    //Check to see if the item already exists in the list
                    var item = items.Where(x => x.SOPNUMBE == detail.orderId && x.LNITMSEQ == lnitmseq).FirstOrDefault();
                    if (item == null)
                    {
                        var line = new taSopLineIvcInsert_ItemsTaSopLineIvcInsert
                        {
                            SOPTYPE = (short)GpSopType.Order,
                            SOPNUMBE = detail.orderId,
                            CUSTNMBR = _shipment.custCode,
                            DOCDATE = ((DateTime)_shipment.dateOrdered).ToShortDateString(),
                            LOCNCODE = detail.olCust2,
                            ITEMNMBR = detail.itemId,
                            UNITPRCE = Convert.ToDecimal(detail.olCust1),
                            QUANTITY = sopLineItem.QUANTITY,
                            //UNITCOST = sopLineItem.UNITCOST,
                            //UNITCOSTSpecified = sopLineItem.UNITCOST > 0 ? true : false,
                            COMMNTID = detail.olCust16,
                            TAXAMNT = sopLineItem.TAXAMNT,
                            QTYONHND = sopLineItem.QTYONHND,
                            QTYRTRND = sopLineItem.QTYRTRND,
                            QTYINUSE = sopLineItem.QTYINUSE,
                            QTYDMGED = sopLineItem.QTYDMGED,
                            LNITMSEQ = lnitmseq,
                            DROPSHIP = sopLineItem.DROPSHIP,
                            QTYTBAOR = sopLineItem.QTYTBAOR,
                            SALSTERR = sopLineItem.SALSTERR,
                            SLPRSNID = sopLineItem.SLPRSNID,
                            ITMTSHID = sopLineItem.ITMTSHID,
                            IVITMTXB = sopLineItem.IVITMTXB,
                            TAXSCHID = sopLineItem.TAXSCHID,
                            PRSTADCD = sopLineItem.PRSTADCD,
                            ShipToName = sopLineItem.ShipToName,
                            CNTCPRSN = sopLineItem.CNTCPRSN,
                            ADDRESS1 = sopLineItem.ADDRESS1,
                            ADDRESS2 = sopLineItem.ADDRESS2,
                            ADDRESS3 = sopLineItem.ADDRESS3,
                            CITY = sopLineItem.CITY,
                            STATE = sopLineItem.STATE,
                            ZIPCODE = sopLineItem.ZIPCODE,
                            COUNTRY = sopLineItem.COUNTRY,
                            PHONE1 = sopLineItem.PHONE1,
                            PHONE2 = sopLineItem.PHONE2,
                            PHONE3 = sopLineItem.PHONE3,
                            FAXNUMBR = sopLineItem.FAXNUMBR,
                            ReqShipDate = ((DateTime)_shipment.shipByDate).ToShortDateString(),
                            FUFILDAT = ((DateTime)_shipment.dateShipped).ToShortDateString(),
                            ACTLSHIP = ((DateTime)_shipment.dateShipped).ToShortDateString(),
                            SHIPMTHD = sopLineItem.SHIPMTHD,
                            QTYCANCE = sopLineItem.QTYCANCE,
                            QTYFULFI = detail.piecesToMove.HasValue ? Convert.ToDecimal(detail.piecesToMove.Value) / GetQtyInBaseUnitOfMeasure(detail.olCust10) : 0,
                            QTYFULFISpecified = true,
                            CMMTTEXT = detail.olCust4,
                            //DEFEXTPRICE = 1,
                            UOFM = detail.olCust5?.ToString(),
                            UpdateIfExists = 1,
                            CURNCYID = Configuration.AppSettings.DefaultCurrency,
                            XTNDPRCE = sopLineItem.XTNDPRCE
                        };

                        if (sopLineItem.MRKDNTYP == 1)
                        {
                            line.MRKDNAMT = sopLineItem.MRKDNAMT;
                            line.MRKDNAMTSpecified = true;
                        }
                        else
                        {
                            line.MRKDNPCT = sopLineItem.MRKDNPCT / 100;
                            line.MRKDNPCTSpecified = true;
                        }
                            
                        items.Add(line);
                    }
                    else
                    {
                        item.QTYFULFI += detail.piecesToMove.HasValue ? Convert.ToDecimal(detail.piecesToMove.Value) / GetQtyInBaseUnitOfMeasure(detail.olCust10) : 0;                        
                    }
                }
            }

            return items.ToArray();
        }

        public taSopLineIvcInsertComponent_ItemsTaSopLineIvcInsertComponent[] GetLineKitItems()
        {
            var items = new List<taSopLineIvcInsertComponent_ItemsTaSopLineIvcInsertComponent>();
            
            foreach (var carton in _shipment.shipmentCartons)
            {
                foreach (var detail in carton.shipmentCartonDetail)
                {
                    //Get the Line Item Sequence First

                    //var lnitmseq = Convert.ToInt32(Math.Truncate(Convert.ToDecimal(detail.orderLine.Value / 16384)) * 16384);
                    //var cmpntseq = (detail.orderLine.Value - lnitmseq) * 16384;

                    var lnitmseq = Convert.ToInt32(detail.olCust29);
                    var cmpntseq = Convert.ToInt32(detail.olCust30);

                    if (cmpntseq > 0 && lnitmseq > 0)
                    {
                        SOP10200_DTO componentItem = DynamicsGpDB.GetSalesTransactionLine((short)GpSopType.Order, detail.orderId, cmpntseq, lnitmseq);

                        if (componentItem == null)
                            throw new Exception($"Component Item {detail.itemId} on sales order {detail.orderId} line id {lnitmseq} component sequence {cmpntseq} is missing. ");

                        var item = items.Where(x => x.SOPNUMBE == detail.orderId && x.LNITMSEQ == lnitmseq && x.CMPNTSEQ == cmpntseq).FirstOrDefault();
                        if (item == null)
                        {
                            items.Add(new taSopLineIvcInsertComponent_ItemsTaSopLineIvcInsertComponent
                            {
                                SOPTYPE = (short)GpSopType.Order,
                                SOPNUMBE = detail.orderId,
                                LNITMSEQ = lnitmseq,
                                CMPNTSEQ = cmpntseq,
                                LOCNCODE = detail.olCust2,
                                ITEMNMBR = detail.itemId,
                                QUANTITY = componentItem.QUANTITY,
                                QTYTBAOR = componentItem.QTYTBAOR,
                                QTYCANCE = componentItem.QTYCANCE,
                                QTYFULFI = detail.piecesToMove.HasValue ? Convert.ToDecimal(detail.piecesToMove.Value) / GetQtyInBaseUnitOfMeasure(detail.olCust10) : 0,
                                QTYFULFISpecified = true,
                                QTYONHND = componentItem.QTYONHND,
                                QTYRTRND = componentItem.QTYRTRND,
                                QTYINUSE = componentItem.QTYINUSE,
                                QTYINSVC = componentItem.QTYINSVC,
                                QTYDMGED = componentItem.QTYDMGED,
                                CUSTNMBR = _shipment.custCode,
                                CMPITUOM = detail.olCust5?.ToString(),
                                CURNCYID = Configuration.AppSettings.DefaultCurrency,
                                UpdateIfExists = 1,
                            });
                        }
                        else
                        {
                            item.QTYFULFI += detail.piecesToMove.HasValue ? Convert.ToDecimal(detail.piecesToMove.Value) / GetQtyInBaseUnitOfMeasure(detail.olCust10) : 0;
                        }
                    }
                }
            }

            return items.ToArray();
        }

        private decimal GetQtyInBaseUnitOfMeasure(string qtyInBaseUnitOfMeasureString)
        {
            decimal qtyInBaseUnitOfMeasure = 1;
            if(!string.IsNullOrWhiteSpace(qtyInBaseUnitOfMeasureString))
            {
                if(!decimal.TryParse(qtyInBaseUnitOfMeasureString, out qtyInBaseUnitOfMeasure))
                    qtyInBaseUnitOfMeasure = 1;
            }
            return qtyInBaseUnitOfMeasure;
        }
    }
}
