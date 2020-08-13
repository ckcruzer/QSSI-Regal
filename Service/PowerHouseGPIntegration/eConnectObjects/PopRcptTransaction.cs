using Microsoft.Dynamics.GP.eConnect.Serialization;
using BSP.PowerHouse.DynamicsGP.Integration.Extensions;
using BSP.PowerHouse.DynamicsGP.Integration.Model;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects
{
    public class PopRcptTransaction : IEConnectObject
    {
        private readonly ReceiptTransaction _receiptTransaction;
        public PopRcptTransaction(ReceiptTransaction receiptTransaction)
        {
            _receiptTransaction = receiptTransaction;
        }
        public string GetXmlSerializedObject()
        {
            if (_receiptTransaction == null)
                return null;

            // Instantiate an eConnectType schema object
            eConnectType eConnect = new eConnectType();

            POPReceivingsType receipt = new POPReceivingsType();

            receipt.taPopRcptLotInsert_Items = GetReceiptLotLines().ToArray();
            receipt.taPopRcptLineInsert_Items = GetReceiptLineItems().ToArray();
            receipt.taPopRcptHdrInsert = GetReceiptHeader();

            POPReceivingsType[] popReceivingsType = { receipt };

            eConnect.POPReceivingsType = popReceivingsType;

            return eConnect.SerializeObject();
        }

        private taPopRcptHdrInsert GetReceiptHeader()
        {
            var templateLine = _receiptTransaction.InventoryAdjustments.Where(t => !string.IsNullOrWhiteSpace(t.vendorId)).FirstOrDefault();
            var vendorId = string.Empty;
            if(templateLine != null)
            {
                vendorId = templateLine.vendorId;
            }
            DateTime? receiptdate = _receiptTransaction.InventoryAdjustments.Min(l => l.activityDate);
            if (!receiptdate.HasValue)
                receiptdate = DateTime.Now;


            var rcptHeader = new taPopRcptHdrInsert
            {
                POPRCTNM = _receiptTransaction.ReceiptNumber,
                POPTYPE = (int)GpReceiptType.Shipment,
                receiptdate = receiptdate.Value.GpFormattedDate(),
                BACHNUMB = _receiptTransaction.BatchNumber,
                VENDORID = vendorId,
                CREATEDIST = 1,
                AUTOCOST = 1,
            };
            return rcptHeader;
        }
        private List<taPopRcptLineInsert_ItemsTaPopRcptLineInsert> GetReceiptLineItems()
        {
            var rcptLineInsertItems = new List<taPopRcptLineInsert_ItemsTaPopRcptLineInsert>();
            foreach (var line in _receiptTransaction.InventoryAdjustments)
            {
                var qtyShipped = line.pieces.HasValue ? Convert.ToDecimal(line.pieces.Value) : 0;
                decimal unitOfMeasureQtyInBase;
                if(!string.IsNullOrWhiteSpace(line.host1) && decimal.TryParse(line.host1, out unitOfMeasureQtyInBase))
                {
                    if(qtyShipped > 0 && unitOfMeasureQtyInBase > 0)
                    {
                        qtyShipped = qtyShipped / unitOfMeasureQtyInBase;
                    }
                }

                var existingLine = rcptLineInsertItems.Where(l => l.RCPTLNNM == Convert.ToInt32(line.host0)).FirstOrDefault();
                if(existingLine == null)
                {
                    var rcptLineInsertItem = new taPopRcptLineInsert_ItemsTaPopRcptLineInsert
                    {
                        POPTYPE = (int)GpReceiptType.Shipment,
                        POPRCTNM = _receiptTransaction.ReceiptNumber,
                        PONUMBER = line.poId,
                        POLNENUM = Convert.ToInt32(line.host0),
                        ITEMNMBR = line.itemId,
                        VENDORID = line.vendorId,
                        VNDITNUM = !string.IsNullOrWhiteSpace(line.host2) ? line.host2 : string.Empty,
                        RCPTLNNM = Convert.ToInt32(line.host0),
                        QTYSHPPD = qtyShipped,
                        InventoryAccount = !string.IsNullOrWhiteSpace(line.glAccount) ? line.glAccount : string.Empty,
                        AUTOCOST = 1,
                    };
                    rcptLineInsertItems.Add(rcptLineInsertItem);
                }
                else
                {
                    existingLine.QTYSHPPD += qtyShipped;
                }
                
            }
            return rcptLineInsertItems;
        }

        private List<taPopRcptLotInsert_ItemsTaPopRcptLotInsert> GetReceiptLotLines()
        {
            var rcptLotInsertItems = new List<taPopRcptLotInsert_ItemsTaPopRcptLotInsert>();
            foreach (var line in _receiptTransaction.InventoryAdjustments)
            {
                if(string.IsNullOrWhiteSpace(line.lotId))
                {
                    continue;
                }
                var rcptLotInsertItem = new taPopRcptLotInsert_ItemsTaPopRcptLotInsert
                {
                    POPRCTNM = _receiptTransaction.ReceiptNumber,
                    ITEMNMBR = line.itemId,
                    SERLTNUM = line.lotId,
                    SERLTQTY = line.pieces.HasValue ? Convert.ToDecimal(line.pieces.Value) : 0,
                    RCPTLNNM = Convert.ToInt32(line.host0)

                };
                rcptLotInsertItems.Add(rcptLotInsertItem);
            }
            return rcptLotInsertItems;
        }
    }
}
