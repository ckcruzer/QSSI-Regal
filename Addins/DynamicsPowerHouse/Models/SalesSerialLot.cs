using System;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class SalesSerialLot
    {
        public string Bin { get; set; }
        public int ComponentSequence { get; set; }
        public DateTime DateReceived { get; set; }
        public decimal DateSeqNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string ItemNumber { get; set; }
        public int LineItemSequence { get; set; }
        public DateTime ManufactureDate { get; set; }
        public short OverrideSerialLot { get; set; }
        public bool Posted { get; set; }
        public short QtyType { get; set; }
        public string SerialLotNumber { get; set; }
        public decimal SerialLotQty { get; set; }
        public int SerialLotSeqNumber { get; set; }
        public string SopNumber { get; set; }
        public short SopType { get; set; }
        public string TrxSource { get; set; }
        public decimal UnitCost { get; set; }
    }
}
