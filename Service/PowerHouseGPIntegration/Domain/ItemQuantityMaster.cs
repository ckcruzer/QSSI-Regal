
namespace BSP.PowerHouse.DynamicsGP.Integration.Domain
{
    public partial class ItemQuantityMaster
    {
        public string ITEMNMBR { get; set; }
        public string LOCNCODE { get; set; }
        public string BINNMBR { get; set; }
        public short RCRDTYPE { get; set; }
        public string PRIMVNDR { get; set; }
        public byte ITMFRFLG { get; set; }
        public decimal BGNGQTY { get; set; }
        public decimal LSORDQTY { get; set; }
        public decimal LRCPTQTY { get; set; }
        public System.DateTime LSTORDDT { get; set; }
        public string LSORDVND { get; set; }
        public System.DateTime LSRCPTDT { get; set; }
        public decimal QTYRQSTN { get; set; }
        public decimal QTYONORD { get; set; }
        public decimal QTYBKORD { get; set; }
        public decimal QTY_Drop_Shipped { get; set; }
        public decimal QTYINUSE { get; set; }
        public decimal QTYINSVC { get; set; }
        public decimal QTYRTRND { get; set; }
        public decimal QTYDMGED { get; set; }
        public decimal QTYONHND { get; set; }
        public decimal ATYALLOC { get; set; }
        public decimal QTYCOMTD { get; set; }
        public decimal QTYSOLD { get; set; }
        public System.DateTime NXTCNTDT { get; set; }
        public System.DateTime NXTCNTTM { get; set; }
        public System.DateTime LSTCNTDT { get; set; }
        public System.DateTime LSTCNTTM { get; set; }
        public short STCKCNTINTRVL { get; set; }
        public string Landed_Cost_Group_ID { get; set; }
        public string BUYERID { get; set; }
        public string PLANNERID { get; set; }
        public short ORDERPOLICY { get; set; }
        public decimal FXDORDRQTY { get; set; }
        public decimal ORDRPNTQTY { get; set; }
        public short NMBROFDYS { get; set; }
        public decimal MNMMORDRQTY { get; set; }
        public decimal MXMMORDRQTY { get; set; }
        public decimal ORDERMULTIPLE { get; set; }
        public short REPLENISHMENTMETHOD { get; set; }
        public decimal SHRINKAGEFACTOR { get; set; }
        public decimal PRCHSNGLDTM { get; set; }
        public decimal MNFCTRNGFXDLDTM { get; set; }
        public decimal MNFCTRNGVRBLLDTM { get; set; }
        public decimal STAGINGLDTME { get; set; }
        public short PLNNNGTMFNCDYS { get; set; }
        public short DMNDTMFNCPRDS { get; set; }
        public byte INCLDDINPLNNNG { get; set; }
        public byte CALCULATEATP { get; set; }
        public byte AUTOCHKATP { get; set; }
        public byte PLNFNLPAB { get; set; }
        public short FRCSTCNSMPTNPRD { get; set; }
        public decimal ORDRUPTOLVL { get; set; }
        public decimal SFTYSTCKQTY { get; set; }
        public decimal REORDERVARIANCE { get; set; }
        public string PORECEIPTBIN { get; set; }
        public string PORETRNBIN { get; set; }
        public string SOFULFILLMENTBIN { get; set; }
        public string SORETURNBIN { get; set; }
        public string BOMRCPTBIN { get; set; }
        public string MATERIALISSUEBIN { get; set; }
        public string MORECEIPTBIN { get; set; }
        public string REPAIRISSUESBIN { get; set; }
        public short ReplenishmentLevel { get; set; }
        public short POPOrderMethod { get; set; }
        public string MasterLocationCode { get; set; }
        public short POPVendorSelection { get; set; }
        public short POPPricingSelection { get; set; }
        public decimal PurchasePrice { get; set; }
        public byte IncludeAllocations { get; set; }
        public byte IncludeBackorders { get; set; }
        public byte IncludeRequisitions { get; set; }
        public short PICKTICKETITEMOPT { get; set; }
        public byte INCLDMRPMOVEIN { get; set; }
        public byte INCLDMRPMOVEOUT { get; set; }
        public byte INCLDMRPCANCEL { get; set; }
        public int DEX_ROW_ID { get; set; }
    }
}
