namespace Postkit.Infrastructure.Email
{
    public class MailJetSettings
    {
        public string ApiKey { get; set; } = default!;
        public string ApiSecret { get; set; } = default!;
        public string FromEmail { get; set; } = default!;
        public string FromName { get; set; } = default!;
        public Dictionary<string, int> Templates { get; set; } = new();
    }
}
