using Hospital.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Hospital.Services.Implementations
{
    public class EmailService : IEmailService
    {
        public void NotifyAdd(string receiver, string username, string password)
        {
            SendMessage(receiver, "Your clinic account", $"<h4>An account has been created for the clinic:</h4><p>Username: {username}</p><p>Password: {password}</p>");
        }
        public void NotifyChangePassword(string receiver, string username, string password)
        {
            SendMessage(receiver, "Your clinic account", $"<h4>Your password has been changed:</h4><p>Username: {username}</p><p>Password: {password}</p>");
        }

        public void NotifyDelete(string receiver, string username)
        {
            SendMessage(receiver, "Your clinic account", $"<p>Your clinic account ({username}) has been deleted</p>");
        }

        public void NotifyUpdate(string receiver, string username)
        {
            SendMessage(receiver, "Your clinic account", $"<p>Your clinic account ({username}) has been updated</p>");
        }

        public void SendMessage(string receiver, string subject, string message)
        {
            MailAddress from = new MailAddress("hospital.project2707@gmail.com", "Hospital");
            MailAddress to = new MailAddress(receiver);
            MailMessage mailMessage = new MailMessage(from, to)
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("hospital.project2707@gmail.com", "hospital_270721"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            smtp.Send(mailMessage);
        }
    }
}
