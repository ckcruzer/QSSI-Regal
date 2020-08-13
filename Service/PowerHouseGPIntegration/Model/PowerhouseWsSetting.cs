using BSP.PowerHouse.DynamicsGP.Integration.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP.PowerHouse.DynamicsGP.Integration.Model
{
    public class PowerhouseWsSetting
    {
        public string USERID { get; set; }
        public string BSPPowerhousePassword { get; set; }
        public string URL { get; set; }
        public string Owner { get; set; }
        public string BSPSOPBatchID { get; set; }
        public string BSPGPWSURL { get; set; }
        
        public string CURNCYID { get; set; }
        public int BSPSOLineQtyToUse { get; set; }

        public int BSPFrequency { get; set; }


        public string BSPSOPFulfillmentBatchID { get; set; }

        public string CMPNYNAM { get; set; }

        public string BSPPHPOReceivingType { get; set; }
        public string BSPPHContReceivingType { get; set; }

        public BatchFrequency BSPBatchFrequency { get; set; }

        public string BSPRecvTrxBatchID { get; set; }

        public string BSPRcvInTransferSite { get; set; }

        public string BSPRcvInTransferToSite { get; set; }

    }
}
