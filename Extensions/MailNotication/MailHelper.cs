using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace MailNotication
{
    public class MailHelper
    {
        private static bool SendMailViaParam(EmailParam context)
        {
            EmailParam param = context as EmailParam;
            try
            {
                if (param.body != "")
                {
                    SmtpClient client = new SmtpClient(param.SMTPServer, param.Port);
                    client.EnableSsl = param.enableSsl;
                    //client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(param.fromEmail, param.fromPassword);

                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    MailAddress fromAddress = new MailAddress(param.fromEmail, param.fromName);
                    MailAddress toAddress = new MailAddress(param.toEmail, param.toName);
                    MailMessage message = new MailMessage(fromAddress, toAddress);
                    message.Body = param.body;
                    message.Subject = param.subject;
                    message.IsBodyHtml = param.IsBodyHtml;
                    message.BodyEncoding = Encoding.UTF8;

                    try
                    {
                        client.Send(message);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Send Email.
        /// </summary>
        /// <param name="subject">subject</param>
        /// <param name="body">body</param>
        /// <returns></returns>
        public static bool SendEmail(string subject, string body)
        {
            EmailParam param = new EmailParam();
            param.SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
            param.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
            param.fromEmail = ConfigurationManager.AppSettings["FromEmail"];
            param.fromName = ConfigurationManager.AppSettings["FromName"];
            param.fromPassword = ConfigurationManager.AppSettings["FromPassWord"];
            param.toEmail = ConfigurationManager.AppSettings["ToEmail"];
            param.toName = ConfigurationManager.AppSettings["ToName"];
            param.subject = subject;
            param.body = body;
            param.IsBodyHtml = true;
            param.enableSsl = true;
            return SendMailViaParam(param);
        }

        public class EmailParam
        {
            public string SMTPServer;
            public int Port;
            public string fromEmail;
            public string fromName;
            public string fromPassword;
            public string toEmail;
            public string toName;
            public string subject;
            public string body;
            public bool enableSsl;
            public bool IsBodyHtml;
        }
    }
}