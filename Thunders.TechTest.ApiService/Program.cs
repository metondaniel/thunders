using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using Thunders.TechTest.ApiService;
using Thunders.TechTest.OutOfBox.Database;
using Thunders.TechTest.OutOfBox.Queues;
using Microsoft.Data.SqlClient;
using System.Data;
using Rebus.Config;
using Thunders.TechTest.ApiService.Application.Queries;
using Thunders.TechTest.ApiService.Infrastructure.Messaging;
using System.Data.SqlClient;
using Dapper;
using Thunders.TechTest.ApiService.Middleware;
using Thunders.TechTest.ApiService.Infrastructure;
using Thunders.TechTest.ApiService.Application.Messages;
using Thunders.TechTest.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// Configuração do OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation())
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddSource("TollSystem.Activity"));

var features = Features.BindFromConfiguration(builder.Configuration);

// Add services to the container.
builder.Services.AddProblemDetails();


var subscriptionBuilder = new SubscriptionBuilder()
        .Add<TollUsage>()             
        .Add<GenerateReportMessage>();

builder.Services.AddBus(
    configuration: builder.Configuration,
    subscriptionBuilder: subscriptionBuilder
);

// Registrar o MessageSender customizado
builder.Services.AddScoped<IMessageSender, RebusMessageSender>();

builder.Services.AddSqlServerDbContext<AppDbContext>(builder.Configuration);

// Registro dos Handlers
builder.Services.AutoRegisterHandlersFromAssemblyOf<TollUsageMessageHandler>();
builder.Services.AutoRegisterHandlersFromAssemblyOf<GenerateReportMessageHandler>();

// Serviços de Aplicação
builder.Services.AddScoped<IReportQueries, ReportQueries>();

// Health Checks para monitorar o banco
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("ThundersTechTestDb"));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Toll System API", Version = "v1" });
});

var app = builder.Build();

// Middleware Pipeline
app.UseMiddleware<GlobalExceptionHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Toll System API V1");
    });
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
app.UseExceptionHandler();

app.MapDefaultEndpoints();

app.MapControllers();

app.Run();
