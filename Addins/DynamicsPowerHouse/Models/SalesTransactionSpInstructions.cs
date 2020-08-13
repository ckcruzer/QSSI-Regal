namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class SalesTransactionSpInstructions
    {
        public string SopNumber { get; set; }
        public short SopType { get; set; }
        public short BarCodeRequired { get; set; }
        public bool CountyOfOriginLabel { get; set; }
        public string CustomerNumber { get; set; }
        public short LabelType { get; set; }
        public bool LabesOnSide { get; set; }
        public decimal MaxNumOfCartons { get; set; }
        public decimal MaxWeight { get; set; }
        public bool NoCompanyMarking { get; set; }
        public bool NoMixedItems { get; set; }
        public bool NoMixLots { get; set; }
        public bool NoMoreThanNLots { get; set; }
        public short NumberOfLots { get; set; }
        public bool OnlyOnePackingList { get; set; }
        public short PackListLocation { get; set; }
        public decimal Qty { get; set; }
        public decimal SkidHeightRequirement { get; set; }
        public string SpecialLabelText { get; set; }
        public string UOfM { get; set; }
        public bool UseWoodPallet { get; set; }
    }
}
