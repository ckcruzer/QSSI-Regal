using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSP.DynamicsGP.PowerHouse.Configuration
{
    public static class AppSettings
    {
        private static readonly NameValueCollection _appSettings = ConfigurationManager.AppSettings;

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
        public static string EmailAttachment { get { return GetAppSetting("EmailAttachment", string.Empty); } }


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
