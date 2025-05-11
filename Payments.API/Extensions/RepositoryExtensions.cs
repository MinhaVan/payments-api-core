using System;
using System.Linq;
using Payments.Data.APIs;
using Payments.Data.Implementations;
using Payments.Data.Repositories;
using Payments.Domain.Interfaces.APIs;
using Payments.Domain.Interfaces.Repositories;
using Payments.Domain.Interfaces.Repository;
using Payments.Domain.Models;
using Payments.Application.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Payments.API.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddCustomRepository(
        this IServiceCollection services,
        SecretManager secretManager)
    {
        services.AddScoped<IUserContext, UserContext>();

        // APIs
        services.AddScoped<IApiAsaas, ApiAsaas>();

        // Repositories
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IBaseRepository<Pagamento>, BaseRepository<Pagamento>>();
        services.AddScoped<IBaseRepository<Plano>, BaseRepository<Plano>>();
        services.AddScoped<IBaseRepository<Assinatura>, BaseRepository<Assinatura>>();
        services.AddScoped<IRedisRepository, RedisRepository>();

        services.AddQueue(secretManager);

        Console.WriteLine("Configuração de repository realizada com sucesso!");

        return services;
    }

    public static IServiceCollection AddQueue(this IServiceCollection services, SecretManager secretManager)
    {
        var connection = secretManager.ConnectionStrings.RabbitConnection.Split(':');

        // services.AddSingleton(sp =>
        //     new ConnectionFactory
        //     {
        //         HostName = connection.ElementAt(0), //"localhost",
        //         Port = int.Parse(connection.ElementAt(1)), // 5672,
        //         UserName = connection.ElementAt(2), // admin
        //         Password = connection.ElementAt(3) // admin
        //     }
        // );

        services.AddScoped<IRabbitMqRepository, RabbitMqRepository>();

        return services;
    }
}