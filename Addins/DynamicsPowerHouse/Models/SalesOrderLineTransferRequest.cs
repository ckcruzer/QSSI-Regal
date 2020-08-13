using System;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class SalesOrderLineTransferRequest
    {
        public short SopType { get; set; }
        public string SopNumber { get; set; }
        public int ComponentSequence { get; set; }
        public int LineItemSequence { get; set; }
        public string VersionNumber { get; set; }
        public DateTime RequestedShipDate { get; set; }
        public decimal Qty { get; set; }
        public decimal QtyAllocated { get; set; }
        public decimal QtyPrevInvoiced { get; set; }
        public string RequestedBy { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime TimeSent { get; set; }
    }
}
