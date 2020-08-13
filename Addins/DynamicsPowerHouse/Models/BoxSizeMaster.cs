using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class BoxSizeMaster
    {
        public string BoxSizeId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedUserId { get; set; }

        public decimal CubicFeet { get; set; }

        public string Description { get; set; }

        public decimal Height { get; set; }

        public bool HouseBoxFlag { get; set; }

        public decimal Length { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedUserId { get; set; }

        public decimal NoteIndex { get; set; }
        public decimal Width { get; set; }
    }
}
