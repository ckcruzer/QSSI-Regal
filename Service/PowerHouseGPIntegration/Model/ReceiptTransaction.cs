using BSP.PowerHouse.DynamicsGP.Integration.PowerHouseWS;
using System.Collections.Generic;

namespace BSP.PowerHouse.DynamicsGP.Integration.Model
{
    public class ReceiptTransaction
    {
        public ReceiptTransaction()
        {
            InventoryAdjustments = new List<InventoryAdjustment>();
        }
        public string ReceiptNumber { get; set; }
        public string PoNumber { get; set; }
        public string BatchNumber { get; set; }
        public List<InventoryAdjustment> InventoryAdjustments { get; set; }
    }
}
