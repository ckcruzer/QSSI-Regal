using BSP.PowerHouse.DynamicsGP.Integration.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP.PowerHouse.DynamicsGP.Integration.eConnectObjects
{
    public interface IEConnectIVObject : IEConnectObject
    {
        List<ItemSite> ItemSites { get;}
    }
}
