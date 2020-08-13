using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;

namespace BSP.PowerHouse.DynamicsGP.Integration.Configuration
{
    public static class AppSettings
    {
        private static readonly NameValueCollection _appSettings = ConfigurationManager.AppSettings;

        private static string _gpConnectionString = ConfigurationManager.ConnectionStrings["GPConnectionString"].ConnectionString;

        public static string GPConnectionString { get { return _gpConnectionString; } }
        public static bool TracingEnabled { get { return GetAppSetting("TracingEnabled", true); } }

        public static int GPCompanyID { get { return GetAppSetting("GPCompanyID", -1); } }
        public static string GPCompanyName { get { return GetAppSetting("GPCompanyName", string.Empty); } }

        public static string PowerhouseEndpoint { get { return GetAppSetting("PowerHouseEndpointName", string.Empty); } }

        public static bool SendShipResponse {  get { return GetAppSetting("BSPSendShipResp", false); } }

        public static string DefaultCurrency { get { return GetAppSetting("DefaultCurrency", "Z-US$"); } }

        public static string InterID { get { return GetAppSetting("InterID", string.Empty); } }

        #region Helper Methods
        private static T GetAppSetting<T>(string searchKey, T defaultValue)
        {
            if (_appSettings.AllKeys.Any(key => key == searchKey))
            {
                try
                {       // see if it can be converted
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                    {
                        defaultValue = (T)converter.ConvertFromString(_appSettings.GetValues(searchKey).First());
                    }
                }
                catch { } // nothing to do, just return default
            }
            return defaultValue;
        } 
        #endregion
    }
}
