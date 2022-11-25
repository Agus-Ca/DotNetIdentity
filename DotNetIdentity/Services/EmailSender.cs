using DotNetIdentity.Models;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;

namespace DotNetIdentity.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    public MailJetOptions mailJetOptions;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        mailJetOptions = _configuration.GetSection("MailJet").Get<MailJetOptions>();

        MailjetClient client = new MailjetClient(mailJetOptions.ApiKey, mailJetOptions.SecretKey)
        {
            Version = ApiVersion.V3_1,
        };
        MailjetRequest request = new MailjetRequest
        {
            Resource = Send.Resource,
        }
        .Property(Send.Messages, new JArray 
        {
            new JObject 
            {
                {
                    "From",
                    new JObject 
                    {
                        {"Email", "agu.e.casado@gmail.com"},
                        {"Name", "Agustín"}
                    }
                }, 
                {
                    "To",
                    new JArray 
                    {
                        new JObject 
                        {
                            email,
                            {"Name", "Estimado/a" }
                        }
                    }
                },
                {
                   "Subject",
                   subject
                },
                {
                    "HTMLPart",
                    htmlMessage
                }
            }
        });

        await client.PostAsync(request);
    }
}