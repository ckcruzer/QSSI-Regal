using BSP.PowerHouse.DynamicsGP.Integration.Extensions;
using BSP.PowerHouse.DynamicsGP.Integration.Model;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects
{
    public class InventoryItemSite : IEConnectObject
    {
        private List<ItemSite> _itemSites;
        public InventoryItemSite(List<ItemSite> itemSites)
        {
            _itemSites = itemSites;
        }
        public string GetXmlSerializedObject()
        {
            if (_itemSites == null)
                return null;

            // Instantiate an eConnectType schema object
            eConnectType eConnect = new eConnectType();

            eConnect.IVInventoryItemSiteType = new IVInventoryItemSiteType[]
            {
                new IVInventoryItemSiteType
                {
                    taItemSite_Items = GetInventoryItemSite()
                }
            };

            return eConnect.SerializeObject();
        }

        private taItemSite_ItemsTaItemSite[] GetInventoryItemSite()
        {
            var taItemSites = new List<taItemSite_ItemsTaItemSite>();
            foreach (var item in _itemSites)
            {
                taItemSites.Add(new taItemSite_ItemsTaItemSite
                {
                    ITEMNMBR = item.ItemNumber,
                    LOCNCODE = item.SiteId
                });
            }
            return taItemSites.ToArray();
        }
    }
}
