using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Core.Data.Context;
using System;
using Core.Service.Configuration;

namespace Core.API.Extensions;
public static class DbContextExtensions
{
    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, SecretManager secretManager)
    {
        var connectionString = secretManager.ConnectionStrings.DefaultConnection;
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("A string de conexão não foi fornecida.");
        }

        services.AddDbContext<APIContext>(options =>
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Core.API"));
        });

        Console.WriteLine("Configuração de bancos de dados realizada com sucesso!");

        return services;
    }
}
