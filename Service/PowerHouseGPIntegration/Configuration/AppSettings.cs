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

        public static string FromEmailAddress { get { return GetAppSetting("FromEmailAddress", string.Empty); } }

        public static string ToEmailAddress { get { return GetAppSetting("ToEmailAddress", string.Empty); } }
        public static string BCCEmailAddress { get { return GetAppSetting("BCCEmailAddress", string.Empty); } }
        public static string CCEmailAddress { get { return GetAppSetting("CCEmailAddress", string.Empty); } }
        public static string EmailSubject { get { return GetAppSetting("EmailSubject", string.Empty); } }
        public static string EmailPort { get { return GetAppSetting("EmailPort", string.Empty); } }
        public static string EmailSSL { get { return GetAppSetting("EmailSSL", string.Empty); } }
        public static string EmailUserName { get { return GetAppSetting("EmailUserName", string.Empty); } }
        public static string EmailPassword { get { return GetAppSetting("EmailPassword", string.Empty); } }
        public static string EmailHost { get { return GetAppSetting("EmailHost", string.Empty); } }

        public static bool AutoPostInventoryTrx { get { return GetAppSetting("AutoPostInventoryTrx", true); } }

        public static bool GPSBAUseDefaultCredentials { get { return GetAppSetting("GPSBAUseDefaultCredentials", true); } }
        public static string GPSBAUserId { get { return GetAppSetting("GPSBAUserId", string.Empty); } }
        public static string GPSBAPassword { get { return GetAppSetting("GPSBAPassword", string.Empty); } }
        public static string GPSBADomain { get { return GetAppSetting("GPSBADomain", string.Empty); } }


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
