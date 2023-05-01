using System;
using System.Collections.Generic;
using System.Linq;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class SalesTransaction
    {
        public SalesTransaction()
        {
            Items = new List<SalesTransactionLine>();
        }
        public string SopNumber { get; set; }
        public short SopType { get; set; }
        public string DocumentId { get; set; }
        public string OriginalNumber { get; set; }
        public short OriginalType { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPoNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime RequestedShipDate { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public string SalespersonId { get; set; }
        public string ShipToName { get; set; }
        public decimal Subtotal { get; set; }
        public Address BillToAddress { get; set; }
        public Address ShipToAddress { get; set; }
        public bool ShipComplete { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string BatchNumber { get; set; }
        public SalesUserDefined UserDefined { get; set; }
        
        public List<SalesTransactionLine> Items { get; set; }
        public string PrimaryShiptoAddressCode { get; set; }
        public string PrimaryBilltoAddressCode { get; set; }
        public DateTime? LineRequestedShipDate => Items.Max(i => i.RequestedShipDate);

        public string SiteID { get; set; }

        public CommentMaster CommentRecord { get; set; }
        public RecordNotesMaster RecordNotes { get; set; }

        public string TaxRegistrationNumber { get; set; }
        public string TaxExempt1 { get; set; }
        public string TaxExempt2 { get; set; }

        public Customer Customer { get; set; }

        public int MasterNumber { get; set; }
    }
}
