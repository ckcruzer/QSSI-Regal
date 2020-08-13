using System;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    [Serializable]
    public class BspItemMaster
    {
        public string BspMaterial { get; set; }
        public string BspMaterialSupplier { get; set; }
        public bool BspBubbleWrap { get; set; }
        public string BspMfgCountry { get; set; }
        public bool BspPackNested { get; set; }
        public string BspPocmnt { get; set; }
        public string BspResinTradeName { get; set; }
        public string BspSampleLabelMaterial { get; set; }
        public bool BspWarnForNewMaterial { get; set; }
        public short BspWarning { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ItemNumber { get; set; }
        public decimal NoteIndex2 { get; set; }
        public DateTime StartDate { get; set; }
        public string UserId { get; set; }
    }
}
