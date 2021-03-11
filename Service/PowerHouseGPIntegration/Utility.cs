using System;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceProcess;
using System.Web;
using System.Xml.Serialization;

namespace BSP.PowerHouse.DynamicsGP.Integration
{
    public class Utility
    {
        #region Utils

        public static bool IsRequiredServiceRunning()
        {
            var services = ServiceController.GetServices().ToList();

            var gpService = services.Where(s => s.ServiceName.Equals("GPService", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (gpService != null)
            {
                if (gpService.Status != ServiceControllerStatus.Running)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            var gpDexServiceControl = services.Where(s => s.ServiceName.Equals("GPDexterityServiceControl", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (gpDexServiceControl != null)
            {
                if (gpDexServiceControl.Status != ServiceControllerStatus.Running)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            var gpDexService = services.Where(s => s.ServiceName.Equals("GPDS-DEFAULT-14", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (gpDexService != null)
            {
                if (gpDexService.Status != ServiceControllerStatus.Running)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }


            return true;

        }

        public static void StartRequiredServices()
        {
            var services = ServiceController.GetServices().ToList();

            var gpDexService = services.Where(s => s.ServiceName.Equals("GPDS-DEFAULT-14", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (gpDexService != null)
            {
                if (gpDexService.Status == ServiceControllerStatus.Stopped)
                {
                    gpDexService.Start();
                }
            }

            var gpDexServiceControl = services.Where(s => s.ServiceName.Equals("GPDexterityServiceControl", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (gpDexServiceControl != null)
            {
                if (gpDexServiceControl.Status == ServiceControllerStatus.Stopped)
                {
                    gpDexServiceControl.Start();
                }
            }

            var gpService = services.Where(s => s.ServiceName.Equals("GPService", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (gpService != null)
            {
                if (gpService.Status == ServiceControllerStatus.Stopped)
                {
                    gpService.Start();
                }
            }
        }

        #endregion


        public static string GetEconnectConnectionString(string connectionString)
        {
            string connStr = connectionString;
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                try
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connStr);
                    if (!builder.IntegratedSecurity)
                    {
                        builder.Remove("User ID");
                        builder.Remove("Password");
                        builder.IntegratedSecurity = true;
                        builder.PersistSecurityInfo = false;
                        connStr = builder.ConnectionString.Replace("Integrated Security=True", "Integrated Security=SSPI");
                    }
                }
                catch (Exception)
                {
                }
            }
            return connStr;
        }
        public static void LogResult<T>(T obj, string fileName = "result")
        {
            var writer = new XmlSerializer(obj.GetType());
            var path = AppDomain.CurrentDomain.BaseDirectory + "//TraceLog//" + fileName + "_" + DateTime.Now.Ticks + ".xml";
            System.IO.FileStream file = System.IO.File.Create(path);

            writer.Serialize(file, obj);
            file.Close();
        }
        public static string GetErrorDescription(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                return errorMessage;

            var startIndex = errorMessage.IndexOf("Error Description = ") + "Error Description = ".Length;
            var endIndex = errorMessage.IndexOf("Node Identifier");

            if (startIndex < 0 || endIndex < 0 || startIndex >= endIndex)
                return errorMessage;

            return errorMessage.Substring(startIndex, endIndex - startIndex).Replace(Environment.NewLine, string.Empty);
        }
        public static string UrlEncodeValue(string value)
        {
            return HttpUtility.UrlEncode(value).Replace("+", @"%20");
        }
        

    }
}
