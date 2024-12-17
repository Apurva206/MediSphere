namespace MediSphere.Services
{
    public interface IEmailService
    {
        void SendEmail(List<string> receiverEmailAddressList, string subject, string body);
    }
}
