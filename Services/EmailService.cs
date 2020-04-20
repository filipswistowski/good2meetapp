using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using activitiesapp.Helpers;
using activitiesapp.Services.Interfaces;

namespace activitiesapp.Services
{
    public class EmailService : IEmailService
    {
        public void SendEmail(MailModel mailModel)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(mailModel.To);
            mail.From = new MailAddress(mailModel.From);
            mail.Subject = mailModel.Subject;
            string Body = mailModel.Body;
            mail.Body = Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials =
                new System.Net.NetworkCredential("XXXXXXXXXXXXXXXXXXXX",
                    "XXXXXXXXXXXX"); 
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
    }
}
