using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace ASE
{
    public class EMail
    {
        public static Dictionary<string, string> SendEmail(string to, string toEmail, string subject, string body)
        {
            return SendEmail(to, toEmail, subject, body, null);
        }

        public static Dictionary<string, string> SendEmail(string to, string toEmail, string subject, string body, params Attachment[] attachments)
        {
            return SendEmail(new string[] { to }, new string[] { toEmail }, subject, body, attachments);
        }

        public static Dictionary<string, string> SendEmail(string[] to, string[] toEmail, string subject, string body, params Attachment[] attachments)
        {
            var ret = new Dictionary<string, string>();

            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.EnableSsl = true;

                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress(message.From.Address, System.Configuration.ConfigurationManager.AppSettings["ASE.SiteName"]);
                    for(int i = 0; i < to.Length; i++)
                        message.To.Add(new MailAddress(toEmail[i], to[i], System.Text.Encoding.UTF8));
                    message.Subject = subject;
                    message.IsBodyHtml = true;
                    message.BodyEncoding = System.Text.Encoding.UTF8;

                    message.Body = body;

                    if (attachments != null)
                        foreach (var item in attachments)
                            message.Attachments.Add(item);

                    smtp.Send(message);
                }
            }
            catch (Exception exc)
            {
                ret.Add("exception", exc.Message);
            }

            return ret;
        }
    }
}