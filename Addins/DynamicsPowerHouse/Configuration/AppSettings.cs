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

        public static string GPConnectionString
        {
            get
            {
                try
                {
                    // Retrieve the connection string from appSettings
                    var connectionString = GetAppSetting("GPConnectionString", string.Empty);

                    if (string.IsNullOrEmpty(connectionString))
                    {
                        // Retrieve the configuration file path
                        var configFilePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

                        throw new ConfigurationErrorsException(
                            $"The 'GPConnectionString' is missing or empty. Please ensure it is configured in the application's appSettings section in the configuration file located at: {configFilePath}");
                    }

                    return connectionString;
                }
                catch (ConfigurationErrorsException ex)
                {
                    // Log or handle configuration-specific error
                    Console.WriteLine($"Configuration error: {ex.Message}");
                    throw; // Re-throw the exception for the calling code
                }
                catch (Exception ex)
                {
                    // Handle unexpected exceptions and log details
                    var configFilePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                    Console.WriteLine($"Unexpected error retrieving GPConnectionString: {ex.Message}. Configuration file path: {configFilePath}");

                    throw new ConfigurationErrorsException(
                        $"An unexpected error occurred while retrieving 'GPConnectionString'. Please check the application's appSettings section in the configuration file located at: {configFilePath}", ex);
                }
            }
        }

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
                {
                    // Try converting the value to the specified type
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                    {
                        defaultValue = (T)converter.ConvertFromString(_appSettings.GetValues(searchKey).First());
                    }
                }
                catch
                {
                    // Ignore conversion errors and return the default value
                }
            }
            return defaultValue;
        }
        #endregion
    }
}
