using System.Collections.Generic;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class SalesTransactionLineComment
    {
        public SalesTransactionLineComment()
        {
            Comment = new List<string>();
        }
        public List<string> Comment { get; set; }
        public string CommentText { get; set; }
    }
}
