using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Postkit.Shared.Interfaces.MailJet;

namespace Postkit.Infrastructure.Email
{
    public class MailjetMailService : IMailService
    {
        private readonly MailJetSettings settings;
        private readonly ILogger<MailjetMailService> logger;
        private readonly MailjetClient mailjetClient;


        public MailjetMailService(IOptions<MailJetSettings> options, ILogger<MailjetMailService> logger)
        {
            this.settings = options.Value;
            this.logger = logger;

            mailjetClient = new MailjetClient(
              settings.ApiKey,
              settings.ApiSecret);
        }

        public async Task SendTenantConfirmationEmail(string appName, string adminEmail, string confirmationLink)
        {
            if (!settings.Templates.TryGetValue("TenantEmailConfirmation", out int templateId))
                throw new KeyNotFoundException("Mailjet template 'TenantEmailConfirmation' not found.");

            var request = new MailjetRequest
            {
                Resource = SendV31.Resource
            }
            .Property(Send.Messages, new JArray {
        new JObject {
            {"From", new JObject {
                {"Email", settings.FromEmail},
                {"Name", settings.FromName}
            }},
            {"To", new JArray {
                new JObject {
                    {"Email", adminEmail},
                    {"Name", adminEmail}
                }
            }},
            {"TemplateID", templateId},
            {"TemplateLanguage", true},
            {"Subject", "Welcome to Postkit – Confirm your email to get started"},
            {"Variables", new JObject {
                   {"year", DateTime.Now.Year.ToString()},
                  {"confirmation_link", confirmationLink},
                  {"admin_email", adminEmail},
                  {"app_name", appName}
            }}
        }
            });

            try
            {
                var response = await mailjetClient.PostAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Mailjet send failed: {StatusCode} - {Content}", response.StatusCode, response.GetErrorMessage());
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending confirmation email to {Email}", adminEmail);
            }
        }

        public async Task SendUserConfirmationEmail(string toEmail, string confirmationLink, string appName)
        {
            if (!settings.Templates.TryGetValue("UserEmailConfirmation", out int templateId))
                throw new KeyNotFoundException("Mailjet template 'EmailConfirmation' not found.");

            string subject = $"Welcome to {appName} – Confirm your email to get started";
            string fromName = $"{appName} Team";

            var request = new MailjetRequest
            {
                Resource = SendV31.Resource
            }
            .Property(Send.Messages, new JArray {
        new JObject {
            {"From", new JObject {
                {"Email", settings.FromEmail},
                {"Name", fromName}
            }},
            {"To", new JArray {
                new JObject {
                    {"Email", toEmail},
                    {"Name", toEmail}
                }
            }},
            {"TemplateID", templateId},
            {"TemplateLanguage", true},
            {"Subject", subject},
            {"Variables", new JObject {
                {"name", toEmail},
                {"confirmation_link", confirmationLink},
                {"year", DateTime.Now.Year.ToString() },
                {"app_name", appName }
            }}
        }
            });

            try
            {
                var response = await mailjetClient.PostAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError("Mailjet send failed: {StatusCode} - {Content}", response.StatusCode, response.GetErrorMessage());
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending confirmation email to {Email}", toEmail);
            }
        }
    }
}
