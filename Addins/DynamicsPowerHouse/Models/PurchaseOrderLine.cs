using System;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class PurchaseOrderLine
    {
        public string PoNumber { get; set; }

        public int Ord { get; set; }

        public short PoLineStatus { get; set; }

        public short PoType { get; set; }

        public string ItemNumber { get; set; }

        public string ItemDescription { get; set; }

        public string VendorId { get; set; }

        public string VendorItemNumber { get; set; }

        public string VendorItemDescription { get; set; }

        public string LocationCode { get; set; }

        public string UOfM { get; set; }

        public decimal UOfMQtyInBase { get; set; }

        public decimal QtyOrdered { get; set; }

        public decimal QtyCanceled { get; set; }

        public decimal QtyCommittedInBase { get; set; }

        public decimal QtyUncommittedInBase { get; set; }
        public decimal QtyRemainingToShip { get; set; }

        public int InventoryIndex { get; set; }

        public DateTime RequiredDate { get; set; }

        public DateTime PromisedDate { get; set; }

        public DateTime PromisedShipDate { get; set; }

        public string RequestedBy { get; set; }

        public string CommentId { get; set; }

        public short DocumentType { get; set; }

        public short ItemTrackingOption { get; set; }

        public DateTime ReleaseByDate { get; set; }

        public short LineNumber { get; set; }

        public string InventoryAccountNumber { get; set; }
        public short NonIv { get; set; }
        public int ItemType { get; set; }

        public bool ShouldBeSentToPowerHouse => NonIv == 0 && (ItemType == 1 || ItemType == 2);

        
    }
}
