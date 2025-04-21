using System;
using System.Linq;
using Core.Data.APIs;
using Core.Data.Implementations;
using Core.Data.Repositories;
using Core.Domain.Interfaces.APIs;
using Core.Domain.Interfaces.Repositories;
using Core.Domain.Interfaces.Repository;
using Core.Domain.Models;
using Core.Service.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Core.API.Extensions;

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
        services.AddScoped<IBaseRepository<Rota>, BaseRepository<Rota>>();
        services.AddScoped<IBaseRepository<Endereco>, BaseRepository<Endereco>>();
        services.AddScoped<IBaseRepository<RotaHistorico>, BaseRepository<RotaHistorico>>();
        services.AddScoped<IRotaHistoricoRepository, RotaHistoricoRepository>();
        services.AddScoped<IBaseRepository<Pagamento>, BaseRepository<Pagamento>>();
        services.AddScoped<IBaseRepository<Veiculo>, BaseRepository<Veiculo>>();
        services.AddScoped<IBaseRepository<Plano>, BaseRepository<Plano>>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IBaseRepository<Assinatura>, BaseRepository<Assinatura>>();
        services.AddScoped<IBaseRepository<UsuarioPermissao>, BaseRepository<UsuarioPermissao>>();
        services.AddScoped<IBaseRepository<Permissao>, BaseRepository<Permissao>>();
        services.AddScoped<IBaseRepository<Aluno>, BaseRepository<Aluno>>();
        services.AddScoped<IRedisRepository, RedisRepository>();

        services.AddQueue(secretManager);

        Console.WriteLine("Configuração de repository realizada com sucesso!");

        return services;
    }

    public static IServiceCollection AddQueue(this IServiceCollection services, SecretManager secretManager)
    {
        var connection = secretManager.ConnectionStrings.RabbitConnection.Split(':');

        services.AddSingleton(sp =>
            new ConnectionFactory
            {
                HostName = connection.ElementAt(0), //"localhost",
                Port = int.Parse(connection.ElementAt(1)), // 5672,
                UserName = connection.ElementAt(2), // admin
                Password = connection.ElementAt(3) // admin
            }
        );

        services.AddScoped<IRabbitMqRepository, RabbitMqRepository>();

        return services;
    }
}