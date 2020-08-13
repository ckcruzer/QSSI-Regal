using System;
using System.Collections.Generic;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class SalesUserDefined
    {
        public SalesUserDefined()
        {
            Comment = new List<string>();
        }
        public List<string> Comment { get; set; }
        public string Comment1 { get; set; }
        public string Comment2 { get; set; }
        public string Comment3 { get; set; }
        public string Comment4 { get; set; }
        public string CommentText { get; set; }
        public string SopNumber { get; set; }
        public short SopType { get; set; }
        public string UserDefined1 { get; set; }
        public string UserDefined2 { get; set; }
        public string UserDefined3 { get; set; }
        public string UserDefined4 { get; set; }
        public string UserDefined5 { get; set; }
        public DateTime UserDefinedDate1 { get; set; }
        public DateTime UserDefinedDate2 { get; set; }
        public string UserDefinedTable1 { get; set; }
        public string UserDefinedTable2 { get; set; }
        public string UserDefinedTable3 { get; set; }
    }
}
