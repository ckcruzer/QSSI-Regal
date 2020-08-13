using System;
using System.Linq;

namespace BSP.DynamicsGP.PowerHouse.Models
{
    public class SalesTransactionLine
    {
        public string SopNumber { get; set; }
        public short SopType { get; set; }
        public int ComponentSequence { get; set; }
        public int LineItemSequence { get; set; }
        public string ItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public DateTime RequestedShipDate { get; set; }
        public decimal Qty { get; set; }
        public decimal QtyRemaining { get; set; }
        public decimal QtyOrdered { get; set; }
        public decimal QtyToInvoice { get; set; }
        public decimal QtyFulfilled { get; set; }
        public decimal QtyAllocated { get; set; }
        public decimal QtyCancelled { get; set; }
        public decimal QtyToShip { get; set; }
        public decimal QtyToBackOrder { get; set; }
        public decimal QtyInBaseUOfM { get; set; }
        public decimal QtyPrevInvoiced { get; set; }
        public string LocationCode { get; set; }
        public string UOfM { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ExtendedPrice { get; set; }
        public CustomerItemXref CustomerItem { get; set; }
        public BspItemMaster BspItemMaster { get; set; }
        public CommentMaster LineComment { get; set; }
        public SalesTransactionLineSpInstructions SpecialInstructions { get; set; }
        public SalesOrderLineTransferRequest TransferRequest { get; set; }
        public short DropShip { get; set; }
        public short NonIv { get; set; }
        public int ItemType { get; set; }
        public string CommodityCode { get; set; }
        
        public bool ShouldBeSentToPowerHouse => DropShip == 0 && NonIv == 0 && (ItemType == 1 || ItemType == 3) && QtyRemaining > 0; //Sales Inventory and Kit

        private string _releaseNumber;
        public string ReleaseNum
        {
            get
            {
                if(!string.IsNullOrWhiteSpace(_releaseNumber))
                {
                    return _releaseNumber;
                }
            
                if (TransferRequest == null || string.IsNullOrWhiteSpace(TransferRequest.VersionNumber) || RequestedShipDate != TransferRequest.RequestedShipDate)
                {
                    return RequestedShipDate.ToString("yyyyMMdd");
                }
                else
                {
                    //check if need to be updated
                    if(QtyPrevInvoiced == TransferRequest.QtyPrevInvoiced)
                    {
                        return TransferRequest.VersionNumber;
                    }
                    else
                    {
                        //line was partially fulfilled and generate a new line version number
                        int versionNumber = 1;
                        if (TransferRequest.VersionNumber.Contains('.'))
                        {
                            var version = TransferRequest.VersionNumber.Split('.')[1];
                            if (int.TryParse(version, out versionNumber))
                            {
                                versionNumber++;
                            }
                            
                        }
                        return string.Format("{0}.{1}", RequestedShipDate.ToString("yyyyMMdd"), versionNumber);
                    }

                }
            }
            set
            {
                _releaseNumber = value;
            }
        }
        public bool IsNewRelease => TransferRequest != null && QtyPrevInvoiced != TransferRequest.QtyPrevInvoiced;

    }
}
