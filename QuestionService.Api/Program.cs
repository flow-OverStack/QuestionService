using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using QuestionService.Api;
using QuestionService.Api.Middlewares;
using QuestionService.Api.Settings;
using QuestionService.Application.DependencyInjection;
using QuestionService.Application.Settings;
using QuestionService.BackgroundJobs.DependencyInjection;
using QuestionService.Cache.DependencyInjection;
using QuestionService.Cache.Settings;
using QuestionService.DAL.DependencyInjection;
using QuestionService.Domain.Settings;
using QuestionService.GraphQl.DependencyInjection;
using QuestionService.Grpc.DependencyInjection;
using QuestionService.Grpc.Settings;
using QuestionService.Messaging.DependencyInjection;
using QuestionService.Messaging.Settings;
using QuestionService.Outbox.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KeycloakSettings>(builder.Configuration.GetSection(nameof(KeycloakSettings)));
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(nameof(KafkaSettings)));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));
builder.Services.Configure<BusinessRules>(builder.Configuration.GetSection(nameof(BusinessRules)));
builder.Services.Configure<EntityRules>(builder.Configuration.GetSection(nameof(EntityRules)));
builder.Services.Configure<GrpcHosts>(builder.Configuration.GetSection(nameof(GrpcHosts)));

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

builder.Services.AddAuthenticationAndAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddGraphQl();
builder.Services.AddGrpcClients();
builder.Services.AddMassTransitServices();
builder.Services.AddOutbox();
builder.Services.AddHangfire(builder.Configuration);

builder.Host.AddLogging(builder.Configuration);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddCache();
builder.Services.AddApplication();

builder.AddOpenTelemetry();
builder.Services.AddHealthChecks(builder.Configuration);
builder.Services.AddCors(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseStatusCodePages();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<WarningHandlingMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfire();
app.SetupHangfireJobs();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapHealthChecks("health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
app.UseForwardedHeaders(builder.Configuration);
app.UseCors("DefaultCorsPolicy");

app.UseMiddleware<ClaimsValidationMiddleware>();

if (app.Environment.IsDevelopment()) app.UseSwaggerUI();
app.UseSwagger();

app.MapControllers();
app.UseGraphQl();

await builder.Services.MigrateDatabaseAsync();

app.LogListeningUrls();

await app.RunAsync();

public partial class Program;