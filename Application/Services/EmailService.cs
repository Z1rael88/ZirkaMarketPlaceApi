using Application.Dtos;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Options;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Application.Services;

public class EmailService(IMailjetClient client, IOptions<MailJetOptions> options) : IEmailService
{
    public async Task SendEmailAsync(string toEmail, IEnumerable<Product> products)
    {
        var request = new MailjetRequest
            {
                Resource = Send.Resource
            }
            .Property(Send.FromEmail, "alekseibaggmet8@gmail.com")
            .Property(Send.FromName, "ZirkaMarketPlace")
            .Property(Send.Subject, "Twoje zamówienie!")
            .Property(Send.TextPart, "Twoje zamówienie!")
            .Property(Send.HtmlPart, "<strong>HTML PART</strong>")
            .Property(Send.Recipients, new JArray
            {
                new JObject
                {
                    { "Email", toEmail }
                }
            })
            .Property(Send.MjTemplateID, options.Value.TemplateId)
            .Property(Send.MjTemplateLanguage, true)
            .Property(Send.Vars, new JObject
            {
                { "products", JArray.FromObject(products.Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    photoUrl = p.PhotoUrl
                })) }
            });

        var response = await client.PostAsync(request);
    }
}

