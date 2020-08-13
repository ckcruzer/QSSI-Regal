using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class BoxSizeItemMaster
    {
        public decimal Apw { get; set; }
        public string BoxSizeId { get; set; }
        public int Count { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedUserId { get; set; }
        public string ItemNumber { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedUserId { get; set; }
        public decimal NoteIndex { get; set; }
        public decimal PerItemCube { get; set; }
        public decimal Weight { get; set; }
    }
}
