using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using MediSphere.Models;

namespace MediSphere.Services
{
    public class EmailService : IEmailService
    {
        private IConfiguration _configuration;

        private string _hostEmailAddress = string.Empty;

        private string _hostAppPassword = string.Empty;

        private SmtpClient _smtpClient;

        private MediSphereDbContext _dbContext;

        public EmailService(IConfiguration configuration, MediSphereDbContext dbContext)
        {
            _configuration = configuration;
            _hostEmailAddress = _configuration["EmailService:HostEmailAddress"];
            _hostAppPassword = _configuration["EmailService:HostAppPassword"];
            _smtpClient = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(_hostEmailAddress, _hostAppPassword)
            };
            _dbContext = dbContext;
        }

        public void SendEmail(List<string> receiverEmailAddressList, string subject, string body)
        {
            MailMessage email = new MailMessage();
            email.From = new MailAddress(_hostEmailAddress); ;
            receiverEmailAddressList = receiverEmailAddressList == null ? ["trialofdjango@gmail.com"] : receiverEmailAddressList;
            email.Subject = subject;
            receiverEmailAddressList.ForEach(receiverEmailAddress =>
            {
                email.To.Add(new MailAddress(receiverEmailAddress));
            });
            email.Body = body;

            try
            {
                //Debug.WriteLine($"Email is as follows:");
                //Debug.WriteLine($"Email: {email}");
                //Debug.WriteLine($"To: {email.To}");
                //Debug.WriteLine($"Subject: {email.Subject}");
                //Debug.WriteLine($"Body: {email.Body}");

                _smtpClient.Send(email);
                Debug.WriteLine("\n\nEmail sent successfully!\n\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\n\nError Occured: {ex.Message}, Details: {ex}\n\n");
            }
        }
    }
}
