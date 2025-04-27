using System;
using Payments.Domain.Interfaces.APIs;
using Payments.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Payments.API.Extensions;

public static class HttpExtensions
{
    public static IServiceCollection AddCustomHttp(this IServiceCollection services, SecretManager secretManager)
    {
        var url = secretManager.Asaas.Url;
        var asaasToken = secretManager.Asaas.AcessToken;
        services.AddHttpClient("api-asaas", client =>
        {
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Add("User-Agent", "VanCoreAPI");
            client.DefaultRequestHeaders.Add("access_token", asaasToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(secretManager.URL.AuthAPI));

        Console.WriteLine("Configuração das APIs consumidas realizada com sucesso!");

        return services;
    }
}
