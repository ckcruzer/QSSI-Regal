using System.Collections.Generic;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class ItemMaster
    {
        public ItemMaster()
        {
            UserCategoryValues = new List<string>();
        }
        public string ItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public short ItemType { get; set; }
        public string ItemClassCode { get; set; }
        public short ItemTrackingOption { get; set; }
        public string AlternateItem1 { get; set; }
        public string AlternateItem2 { get; set; }
        public List<string> UserCategoryValues { get; set; }
    }
}
