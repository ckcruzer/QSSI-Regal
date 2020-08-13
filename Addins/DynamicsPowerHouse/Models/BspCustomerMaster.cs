using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class BspCustomerMaster
    {
        public string CustomerNumber { get; internal set; }
        public bool DockToStockOptOut { get; internal set; }
        public string FedexAccountNumber { get; internal set; }
        public string UpsAccountNumber { get; internal set; }
        public decimal LogisticsNoteIndex { get; set; }
        public RecordNotesMaster LogisticsNote { get; set; }
    }
}
