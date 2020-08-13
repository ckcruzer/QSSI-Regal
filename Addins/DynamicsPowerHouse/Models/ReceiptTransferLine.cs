using System;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class ReceiptTransferLine
    {
        public string ReceiptNumber { get; set; }
        public int LineItemSequence { get; set; }
        public int ReceiptLineItem { get; set; }
        public string PoNumber { get; set; }
        public string ItemNumber { get; set; }
        public string ItemDesc { get; set; }
        public string VendorItemNumber { get; set; }
        public string VendorItemDescription { get; set; }
        public decimal UofMQtyInBase { get; set; }
        public DateTime ActualShipDate { get; set; }
        public string CommentID { get; set; }
        public string UofM { get; set; }
        public decimal UnitCost { get; set; }
        public decimal ExtendedCost { get; set; }
        public string LocationCode { get; set; }
        public short NonIv { get; set; }
        public string JobNumber{ get; set; }
        public decimal TaxAmount { get; set; }
        public decimal BackoutTaxAmount { get; set; }
        public int InventoryAccountIndex { get; set; }

        public string InventoryAccount { get; set; }
        public string ShipMethod { get; set; }
        
        public string MappedLocationCode { get; set; }

        public decimal QTYShipped { get; set; }
        public decimal QTYInvoiced { get; set; }
        public DateTime PromisedDate { get; set; }

        public int ItemType { get; set; }

        public bool ShouldBeSentToPowerHouse => NonIv == 0 && (ItemType == 1 || ItemType == 3); //Sales Inventory and Kit
    }
}
