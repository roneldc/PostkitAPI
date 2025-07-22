using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Postkit.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Identity.Services
{
    public class MailjetMailService : IMailService
    {
        private readonly IConfiguration config;
        private readonly ILogger<MailjetMailService> logger;
        private readonly MailjetClient mailjetClient;


        public MailjetMailService(IConfiguration config, ILogger<MailjetMailService> logger)
        {
            this.config = config;
            this.logger = logger;

            mailjetClient = new MailjetClient(
              config["MailJet:ApiKey"],
              config["MailJet:ApiSecret"]);
        }

        public async Task SendConfirmationEmail(string toEmail, string name, string confirmationLink, string appName)
        {
            string subject = $"Welcome to {appName} – Confirm your email to get started";

            var request = new MailjetRequest
            {
                Resource = SendV31.Resource
            }
            .Property(Send.Messages, new JArray {
            new JObject {
                {"From", new JObject {
                    {"Email", config["MailJet:FromEmail"]},
                    {"Name", config["MailJet:FromName"]}
                }},
                {"To", new JArray {
                    new JObject {
                        {"Email", toEmail},
                        {"Name", name}
                    }
                }},
                {"TemplateID", long.Parse(config["MailJet:TemplateId"]!)},
                {"TemplateLanguage", true},
                {"Subject", subject},
                {"Variables", new JObject {
                    {"name", name},
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
