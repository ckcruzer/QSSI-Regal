using BSP.PowerHouse.DynamicsGP.Integration.Configuration;
using BSP.PowerHouse.DynamicsGP.Integration.Extensions;
using BSP.PowerHouse.DynamicsGP.Integration.Model;
using BSP.PowerHouse.DynamicsGP.Integration.PowerHouseWS;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using System;
using System.Collections.Generic;

namespace BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects
{
    public class IVInventoryTransfer : IEConnectIVObject
    {
        private readonly string _batchNumber;
        private readonly InventoryAdjustment _inventoryAdjustment;
        private readonly PowerhouseWsSetting _powerhouseWsSetting;

        public List<ItemSite> ItemSites { get; }
        public IVInventoryTransfer(string batchNumber, InventoryAdjustment inventoryAdjustment, PowerhouseWsSetting powerhouseWsSetting)
        {
            _batchNumber = batchNumber;
            _inventoryAdjustment = inventoryAdjustment;
            _powerhouseWsSetting = powerhouseWsSetting;

            ItemSites = new List<ItemSite>();
        }
        public string GetXmlSerializedObject()
        {
            if (_inventoryAdjustment == null)
                return null;

            // Instantiate an eConnectType schema object
            eConnectType eConnect = new eConnectType();

            IVInventoryTransferType inventoryTransfer = new IVInventoryTransferType();

            inventoryTransfer.taIVTransferHeaderInsert = GetTrxHeader();
            inventoryTransfer.taIVTransferLineInsert_Items = GetTrxLines();
            if (!string.IsNullOrWhiteSpace(_inventoryAdjustment.lotId))
                inventoryTransfer.taIVTransferLotInsert_Items = GetTrxLineLots();

            IVInventoryTransferType[] iVInventoryTransferType = { inventoryTransfer };

            eConnect.IVInventoryTransferType = iVInventoryTransferType;

            return eConnect.SerializeObject();
        }
        private taIVTransferLotInsert_ItemsTaIVTransferLotInsert[] GetTrxLineLots() => new taIVTransferLotInsert_ItemsTaIVTransferLotInsert[]
        {
                new taIVTransferLotInsert_ItemsTaIVTransferLotInsert
                {
                    IVDOCNBR = _inventoryAdjustment.ifSeqNum.Value.ToString(),
                    ITEMNMBR = _inventoryAdjustment.itemId,
                    LOCNCODE = _powerhouseWsSetting.BSPRcvInTransferToSite, 
                    SERLTQTY =  Convert.ToDecimal(_inventoryAdjustment.pieces.Value),
                    LOTNUMBR = _inventoryAdjustment.lotId, 

                }
        };

        private taIVTransferLineInsert_ItemsTaIVTransferLineInsert[] GetTrxLines()
        {
            var line = new taIVTransferLineInsert_ItemsTaIVTransferLineInsert
            {
                IVDOCNBR = _inventoryAdjustment.ifSeqNum.Value.ToString(),
                ITEMNMBR = _inventoryAdjustment.itemId,
                TRXLOCTN = _powerhouseWsSetting.BSPRcvInTransferSite,
                TRNSTLOC = _powerhouseWsSetting.BSPRcvInTransferToSite,
                TRXQTY = Convert.ToDecimal(_inventoryAdjustment.pieces.Value),
                Reason_Code = _inventoryAdjustment.hostAdjCode 
            };

            //add item site mapping
            ItemSites.Clear();
            ItemSites.Add(new ItemSite { ItemNumber = line.ITEMNMBR, SiteId = line.TRXLOCTN });
            ItemSites.Add(new ItemSite { ItemNumber = line.ITEMNMBR, SiteId = line.TRNSTLOC });

            return new taIVTransferLineInsert_ItemsTaIVTransferLineInsert[] { line };
        }
        private taIVTransferHeaderInsert GetTrxHeader() => new taIVTransferHeaderInsert
        {
            IVDOCNBR = _inventoryAdjustment.ifSeqNum.Value.ToString(),
            BACHNUMB = _batchNumber,
            DOCDATE = _inventoryAdjustment.activityDate.HasValue ? _inventoryAdjustment.activityDate.Value.GpFormattedDate() : DateTime.Now.GpFormattedDate(),
        };
    }
}
