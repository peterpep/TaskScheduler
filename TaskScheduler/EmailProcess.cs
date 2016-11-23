using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace TaskScheduler
{
    class EmailProcess
    {
        private MailMessage mail;
        private SmtpClient client;
        //private string SendTo = "peterpep@umich.edu";
        

        public EmailProcess(string emailFrom, string passwordFrom, string SendTo)
        {
            mail = new MailMessage(emailFrom, SendTo);
            client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.Credentials = new NetworkCredential(emailFrom, passwordFrom);
            
        }

        public void SendMail(string Subject, string Message)
        {
            mail.Subject = Subject;
            mail.Body = Message;
            mail.BodyEncoding = Encoding.UTF8;
            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            client.Send(mail);
        }
        
    }
}
