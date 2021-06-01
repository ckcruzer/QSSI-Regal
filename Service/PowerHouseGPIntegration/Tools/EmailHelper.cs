using BSP.PowerHouse.DynamicsGP.Integration.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BSP.PowerHouse.DynamicsGP.Integration.Tools
{
    public static class EmailHelper
    {
        public static void SendEmail(string pSubject, string pMessage)
        {
            string toEmailAddress = AppSettings.ToEmailAddress;

            SendEmail(pSubject, pMessage, toEmailAddress);
        }

        public static void SendEmail(string pSubject, string pMessage, string pToAddress)
        {
            if (string.IsNullOrEmpty(AppSettings.FromEmailAddress) || string.IsNullOrEmpty(AppSettings.ToEmailAddress))
            {
                return; //skip sending mail if no values present
            }

            var fromAddress = new MailAddress(AppSettings.FromEmailAddress);
            var toAddress = new MailAddress(AppSettings.ToEmailAddress);
            string username = AppSettings.EmailUserName == string.Empty ? fromAddress.Address : AppSettings.EmailUserName;
            string password = AppSettings.EmailPassword;
            string host = AppSettings.EmailHost;
            int port = Convert.ToInt32(AppSettings.EmailPort);
            bool ssl = Convert.ToBoolean(AppSettings.EmailSSL);
            string subject = pSubject;
            string body = pMessage;

            try
            {

                var smtp = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = ssl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, password)
                };

                var message = new MailMessage();
                message.From = fromAddress;
                message.To.Add(pToAddress);
                if (!string.IsNullOrEmpty(AppSettings.BCCEmailAddress))
                    message.Bcc.Add(AppSettings.BCCEmailAddress);
                if (!string.IsNullOrEmpty(AppSettings.CCEmailAddress))
                    message.CC.Add(AppSettings.CCEmailAddress);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;                                              
               
                smtp.Send(message);                
            }
            catch (Exception ex)
            {
                EventLogUtility.LogException(ex);
            }
        }
    }
}
