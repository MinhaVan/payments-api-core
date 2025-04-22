using System;
using Payments.Application.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Payments.API.Extensions;

public static class SecretManagerExtensions
{
    public static SecretManager AddSecretManager(this IServiceCollection services, ConfigurationManager configuration)
    {
        var secretManager = configuration.Get<SecretManager>();
        if (secretManager == null)
        {
            throw new Exception("Erro ao carregar as configurações do Secrets Manager.");
        }
        services.AddSingleton(secretManager);

        Console.WriteLine("Configuração da secret realizada com sucesso!");

        return secretManager;
    }
}
