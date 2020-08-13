
namespace BSP.PowerHouse.DynamicsGP.Integration.Domain
{
    public partial class SalesOrderTrackingInfo
    {
        public string SOPNUMBE { get; set; }
        public short SOPTYPE { get; set; }
        public string DOCID { get; set; }
        public string ORIGNUMB { get; set; }
        public short ORIGTYPE { get; set; }
        public int MSTRNUMB { get; set; }
        public short VOIDSTTS { get; set; }
        public string ID { get; set; }
        public string TrackingNumber { get; set; }
        public string LineItems { get; set; }
        public short Completed { get; set; }
        public bool IsCompleted { get { return Completed == 1; } }
    }
}
