using System;
using Core.Service.Implementations;
using Core.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Core.API.Filters;
using Core.API.Converters;
using FluentValidation.AspNetCore;
using FluentValidation;
using System.Reflection;
using StackExchange.Redis;
using Core.Service.Configuration;
// using Core.Service.Workers;

namespace Core.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services, SecretManager secretManager)
    {
        services.AddHttpContextAccessor();
        services.AddCache(secretManager);

        services.AddScoped<IAmazonService, AmazonService>();
        services.AddScoped<IMotoristaService, MotoristaService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAsaasService, AsaasService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IAlunoService, AlunoService>();
        services.AddScoped<IEnderecoService, EnderecoService>();
        services.AddScoped<IRotaService, RotaService>();
        services.AddScoped<ITrajetoService, TrajetoService>();
        services.AddScoped<IAjusteEnderecoService, AjusteEnderecoService>();
        services.AddScoped<IVeiculoService, VeiculoService>();
        services.AddScoped<IPlanoService, PlanoService>();
        services.AddScoped<IAssinaturaService, AssinaturaService>();


        Console.WriteLine("Configuração das services realizada com sucesso!");

        return services;
    }

    public static IServiceCollection AddCache(this IServiceCollection services, SecretManager secretManager)
    {
        services.AddSignalR().AddStackExchangeRedis(secretManager.ConnectionStrings.RedisConnection, options =>
        {
            options.Configuration.ChannelPrefix = "rotaHub"; // Nome opcional
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = secretManager.ConnectionStrings.RedisConnection;
            return ConnectionMultiplexer.Connect(configuration);
        });

        Console.WriteLine("Configuração do Redis realizada com sucesso!");

        return services;
    }

    public static IServiceCollection AddCustomMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }

    public static IServiceCollection AddControllersWithFilters(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
        });

        services.AddSignalR();

        services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}