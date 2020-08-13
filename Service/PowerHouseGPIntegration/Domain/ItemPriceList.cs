
namespace BSP.PowerHouse.DynamicsGP.Integration.Domain
{
    public partial class ItemPriceList
    {
        public string ITEMNMBR { get; set; }
        public string CURNCYID { get; set; }
        public string PRCLEVEL { get; set; }
        public string UOFM { get; set; }
        public decimal TOQTY { get; set; }
        public decimal FROMQTY { get; set; }
        public decimal UOMPRICE { get; set; }
        public decimal QTYBSUOM { get; set; }
        public System.DateTime DEX_ROW_TS { get; set; }
        public int DEX_ROW_ID { get; set; }
    }
}
