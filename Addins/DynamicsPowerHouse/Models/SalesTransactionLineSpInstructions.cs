namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class SalesTransactionLineSpInstructions
    {
        public short BarCodeRequired { get; set; }
        public bool BubbleWrap { get; set; }
        public string ExcludeLotNumber { get; set; }
        public short LabelType { get; set; }
        public bool NoMixedItems { get; set; }
        public bool NoMixLots { get; set; }
        public bool NoMoreThanNLots { get; set; }
        public short NumberOfLots { get; set; }
        public bool PackNested { get; set; }
        public string SpecialLabelText { get; set; }
        public bool WarnForNewMaterial { get; set; }
        public decimal Qty { get; set; }
        public string UOfM { get; set; }
    }
}
