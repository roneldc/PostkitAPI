namespace Postkit.Shared.Interfaces.MailJet
{
    public interface IMailService
    {
        Task SendUserConfirmationEmail(string toEmail, string confirmationLink, string appName);
        Task SendTenantConfirmationEmail(string appName, string adminEmail, string confirmationLink);
    }
}
