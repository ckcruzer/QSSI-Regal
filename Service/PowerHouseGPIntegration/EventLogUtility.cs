using System.Diagnostics;
using System.Text;

namespace BSP.PowerHouse.DynamicsGP.Integration
{
    public class EventLogUtility
    {
        private const string _constEventLogSource = "PowerHouseDynamicsGPService";
        private const int _constEventLogId = 1001;

        public static void LogException(System.Exception ex)
        {
            LogErrorMessage(FormatExceptionMessage(ex));
        }

        public static void LogInformationMessage(string message)
        {
            EventLog.WriteEntry(_constEventLogSource, SanitizeMessage(message), EventLogEntryType.Information, _constEventLogId);
        }

        public static void LogWarningMessage(string message)
        {
            EventLog.WriteEntry(_constEventLogSource, SanitizeMessage(message), EventLogEntryType.Warning, _constEventLogId);
        }

        public static void LogErrorMessage(string message)
        {
            EventLog.WriteEntry(_constEventLogSource, SanitizeMessage(message), EventLogEntryType.Error, _constEventLogId);
        }
        private static string SanitizeMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message) && message.Length > 10000)
                message = "A string written to the event log cannot exceed 32766 characters:" + message.Substring(0, 10000);

            return message;
        }
        private static string FormatExceptionMessage(System.Exception ex)
        {
            StringBuilder sbExceptionMessage = new StringBuilder();

            sbExceptionMessage.Append("An Exception has been raised:\r\n\r\n");
            sbExceptionMessage.Append("");
            sbExceptionMessage.Append(ex.Message);
            sbExceptionMessage.Append("\r\n");
            sbExceptionMessage.Append(ex.Source);
            sbExceptionMessage.Append("\r\n");
            sbExceptionMessage.Append(ex.StackTrace);

            if (ex.InnerException != null)
            {
                sbExceptionMessage.Append("\r\n\r\nAdditional Exception details:\r\n\r\n");
                sbExceptionMessage.Append(ex.InnerException.Message);
                sbExceptionMessage.Append("\r\n");
                sbExceptionMessage.Append(ex.InnerException.Source);
                sbExceptionMessage.Append("\r\n");
                sbExceptionMessage.Append(ex.InnerException.StackTrace);

                if (ex.InnerException.InnerException != null)
                {
                    sbExceptionMessage.Append("\r\n\r\nAdditional Exception details:\r\n\r\n");
                    sbExceptionMessage.Append(ex.InnerException.InnerException.Message);
                    sbExceptionMessage.Append("\r\n");
                    sbExceptionMessage.Append(ex.InnerException.InnerException.Source);
                    sbExceptionMessage.Append("\r\n");
                    sbExceptionMessage.Append(ex.InnerException.InnerException.StackTrace);
                }
            }
            
            return sbExceptionMessage.ToString();
        }


    }
}
