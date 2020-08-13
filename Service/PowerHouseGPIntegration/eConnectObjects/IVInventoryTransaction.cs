using BSP.PowerHouse.DynamicsGP.Integration.Configuration;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using BSP.PowerHouse.DynamicsGP.Integration.Extensions;
using BSP.PowerHouse.DynamicsGP.Integration.PowerHouseWS;
using System;
using System.Collections.Generic;
using BSP.PowerHouse.DynamicsGP.Integration.Model;

namespace BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects
{
    public class IVInventoryTransaction : IEConnectIVObject
    {
        private readonly string _batchNumber;
        private readonly InventoryAdjustment _inventoryAdjustment;
        private readonly DynamicsGpSettings _dynamicsGpSettings;

        public List<ItemSite> ItemSites { get; }
      
        public IVInventoryTransaction(string batchNumber, InventoryAdjustment inventoryAdjustment, DynamicsGpSettings dynamicsGpSettings)
        {
            _batchNumber = batchNumber;
            _inventoryAdjustment = inventoryAdjustment;
            _dynamicsGpSettings = dynamicsGpSettings;

            ItemSites = new List<ItemSite>();
        }
        public string GetXmlSerializedObject()
        {
            if (_inventoryAdjustment == null)
                return null;

            // Instantiate an eConnectType schema object
            eConnectType eConnect = new eConnectType();

            IVInventoryTransactionType inventoryTrx = new IVInventoryTransactionType();

            inventoryTrx.taIVTransactionHeaderInsert = GetTrxHeader();
            inventoryTrx.taIVTransactionLineInsert_Items = GetTrxLines();
            inventoryTrx.taIVTransactionLotInsert_Items = GetTrxLineLots();

            IVInventoryTransactionType[] ivInventoryTransactionType = { inventoryTrx };

            eConnect.IVInventoryTransactionType = ivInventoryTransactionType;

            return eConnect.SerializeObject();
        }

       
        private taIVTransactionLotInsert_ItemsTaIVTransactionLotInsert[] GetTrxLineLots() => new taIVTransactionLotInsert_ItemsTaIVTransactionLotInsert[]
        {
                new taIVTransactionLotInsert_ItemsTaIVTransactionLotInsert
                {
                    IVDOCTYP = (short)GPIvTrxType.Adjustment,
                    IVDOCNBR = _inventoryAdjustment.ifSeqNum.Value.ToString(),
                    ITEMNMBR = _inventoryAdjustment.itemId,
                    LOCNCODE = string.IsNullOrWhiteSpace(_inventoryAdjustment.holdCodeFrom) 
                    ? (_inventoryAdjustment.requester.Equals("OLD", StringComparison.OrdinalIgnoreCase) ? _dynamicsGpSettings.OldMaterialSiteId : _dynamicsGpSettings.MainSiteId)
                    : (_inventoryAdjustment.requester.Equals("OLD", StringComparison.OrdinalIgnoreCase) ? _dynamicsGpSettings.OldMaterialHoldSiteId : _dynamicsGpSettings.MainHoldSiteId),
                    ADJTYPE = (short)(decimal.Parse(_inventoryAdjustment.signCode + _inventoryAdjustment.pieces.Value) > 0 ? GPIvAdjustmentType.Increase : GPIvAdjustmentType.Decrease),
                    SERLTQTY =  Convert.ToDecimal(_inventoryAdjustment.pieces.Value),
                    LOTNUMBR = _inventoryAdjustment.lotId,
                    AUTOCREATELOT = decimal.Parse(_inventoryAdjustment.signCode + _inventoryAdjustment.pieces.Value) > 0 ? 1 : 0
                }
        };

        private taIVTransactionLineInsert_ItemsTaIVTransactionLineInsert[] GetTrxLines()
        {

            var line = new taIVTransactionLineInsert_ItemsTaIVTransactionLineInsert
            {
                IVDOCTYP = (short)GPIvTrxType.Adjustment,
                IVDOCNBR = _inventoryAdjustment.ifSeqNum.Value.ToString(),
                ITEMNMBR = _inventoryAdjustment.itemId,
                //look for requester - NEW or OLD
                TRXLOCTN = string.IsNullOrWhiteSpace(_inventoryAdjustment.holdCodeFrom)
                ? (_inventoryAdjustment.requester.Equals("OLD", StringComparison.OrdinalIgnoreCase) ? _dynamicsGpSettings.OldMaterialSiteId : _dynamicsGpSettings.MainSiteId)
                : (_inventoryAdjustment.requester.Equals("OLD", StringComparison.OrdinalIgnoreCase) ? _dynamicsGpSettings.OldMaterialHoldSiteId : _dynamicsGpSettings.MainHoldSiteId),
                TRXQTY = decimal.Parse(_inventoryAdjustment.signCode + _inventoryAdjustment.pieces.Value),
                Reason_Code = _inventoryAdjustment.hostAdjCode
            };

            //add item site mapping
            ItemSites.Clear();
            ItemSites.Add(new ItemSite { ItemNumber = line.ITEMNMBR, SiteId = line.TRXLOCTN });

            return new taIVTransactionLineInsert_ItemsTaIVTransactionLineInsert[] { line };
        }
        private taIVTransactionHeaderInsert GetTrxHeader() => new taIVTransactionHeaderInsert
        {
            IVDOCTYP = (short)GPIvTrxType.Adjustment,
            IVDOCNBR = _inventoryAdjustment.ifSeqNum.Value.ToString(),
            BACHNUMB = _batchNumber,
            DOCDATE = _inventoryAdjustment.activityDate.HasValue ? _inventoryAdjustment.activityDate.Value.GpFormattedDate() : DateTime.Now.GpFormattedDate(),
            NOTETEXT = $"Adjustment Code: {_inventoryAdjustment.adjCode}. Created by:{_inventoryAdjustment.usrId}"
        };
    }
}
