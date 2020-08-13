using System;
using System.Collections.Generic;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class SalesOrderTransferRequest
    {
        public SalesOrderTransferRequest()
        {
            Items = new List<SalesOrderLineTransferRequest>();
        }
        public string SopNumber { get; set; }
        public short SopType { get; set; }
        public DateTime RequestedShipDate { get; set; }
        public string RequestedBy { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime TimeSent { get; set; }
        public string ErrorMessageText { get; set; }
        public List<SalesOrderLineTransferRequest> Items { get; set; }

    }
}
