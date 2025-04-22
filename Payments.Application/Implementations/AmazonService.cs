using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Payments.Domain.Interfaces.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payments.Application.Implementations;

public class AmazonService : IAmazonService
{
    private readonly RegionEndpoint _region = RegionEndpoint.USEast1;

    public async Task SendEmail(string destino, string titulo, string mensagem)
        => await SendEmail(new List<string> { destino }, titulo, mensagem);

    public async Task SendEmail(List<string> destinos, string titulo, string mensagem)
    {
        var client = new AmazonSimpleEmailServiceClient(_region);

        var sendRequest = new SendEmailRequest
        {
            Source = "contato@viavan.com.br",
            Destination = new Destination
            {
                ToAddresses = destinos
            },
            Message = new Message
            {
                Subject = new Content(titulo),
                Body = new Body
                {
                    Html = new Content
                    {
                        Charset = "UTF-8",
                        Data = mensagem
                    }
                }
            }
        };

        await client.SendEmailAsync(sendRequest);
    }
}