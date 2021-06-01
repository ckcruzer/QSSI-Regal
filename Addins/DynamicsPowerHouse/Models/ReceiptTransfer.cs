using System;
using System.Collections.Generic;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class ReceiptTransfer
    {
        public ReceiptTransfer()
        {
            Items = new List<ReceiptTransferLine>();
        }
        public string ContainerID { get; set; }        
        public string LocationCode { get; set; }
        public DateTime CreatedDate { get; set; }

        // Added as per Margie's request 09122020
        public DateTime ActualShip { get; set; }

        public List<ReceiptTransferLine> Items { get; set; }

    }
}
