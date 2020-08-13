using System;
using System.Collections.Generic;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class PurchaseOrder
    {
        public PurchaseOrder()
        {
            Items = new List<PurchaseOrderLine>();
        }
        public string PoNumber { get; set; }

        public short PoStatus { get; set; }

        public short PoType { get; set; }

        public string UserToEnter { get; set; }

        public DateTime DocumentDate { get; set; }

        public DateTime LastEditDate { get; set; }

        public DateTime LastPrintedDate { get; set; }

        public DateTime PromisedDate { get; set; }

        public DateTime PromisedShipDate { get; set; }

        public DateTime RequiredDate { get; set; }

        public DateTime RequisitionDate { get; set; }

        public string ShippingMethod { get; set; }

        public string VendorId { get; set; }

        public string VendorName { get; set; }

        public string CustomerNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string CommentId { get; set; }

        public string BuyerId { get; set; }

        public short RevisionNumber { get; set; }

        public short ChangeOrderFlag { get; set; }

        public string PurchaseCompanyName { get; set; }

        public string PurchaseContact { get; set; }

        public string PurchaseAddress1 { get; set; }

        public string PurchaseAddress2 { get; set; }

        public string PurchaseAddress3 { get; set; }

        public string PurchaseCity { get; set; }

        public string PurchaseState { get; set; }

        public string PurchaseZipCode { get; set; }

        public string PurchaseCountryCode { get; set; }

        public string PurchaseCountry { get; set; }

        public string PurchasePhone1 { get; set; }

        public string PurchasePhone2 { get; set; }

        public string PurchasePhone3 { get; set; }

        public string PurchaseFax { get; set; }

        public string LocationCode { get; set; } //Used for the warehouse id mapping

        public List<PurchaseOrderLine> Items { get; set; }

    }
}
