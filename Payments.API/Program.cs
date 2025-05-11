using System;
using System.IO;
using AspNetCoreRateLimit;
using Payments.API.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Payments.Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"Iniciando a API no ambiente '{environment}'");

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Adiciona as configurações do Secrets Manager
var secretManager = builder.Services.AddSecretManager(builder.Configuration);
Console.WriteLine($"Secret: '{JsonConvert.SerializeObject(secretManager)}'");

// Configura os serviços
builder.Services.AddCustomAuthentication(secretManager)
                .AddCustomAuthorization()
                .AddCustomDbContext(secretManager)
                .AddCustomSwagger()
                .AddCustomRateLimiting(secretManager)
                .AddCustomResponseCompression()
                .AddCustomCors()
                .AddCustomServices(secretManager)
                .AddCustomRepository(secretManager)
                .AddCustomMapper()
                .AddControllersWithFilters()
                .AddCustomHttp(secretManager);
// .AddRabbitMq().WithEnviroments(builder.Configuration).WithScope().AddSubscribers(typeof(Program).Assembly);

// Configura o logger
builder.Logging.ClearProviders().AddConsole().AddDebug();

var app = builder.Build();

// Configurações específicas para desenvolvimento
if (environment == "local")
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payments.API v1"));
}
else
{
    using (var scope = app.Services.CreateScope())
    {
        Console.WriteLine($"Rodando migrations '{environment}'");
        var db = scope.ServiceProvider.GetRequiredService<APIContext>();
        db.Database.Migrate();
        Console.WriteLine($"Migrations '{environment}' executadas com sucesso");
    }

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payments.API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseResponseCompression();
app.UseRouting();
app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");
app.MapMetrics();
app.UseWebSockets();
app.MapControllers();

Console.WriteLine("Configuração de API finalizada com sucesso!");

app.Run();