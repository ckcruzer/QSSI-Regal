using System;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class CommentMaster
    {
        public decimal NoteIndex { get; set; }
        public string CommentID { get; set; }
        public string CommentText { get; set; }
        public RecordNotesMaster Notes { get; set; }
    }
}
