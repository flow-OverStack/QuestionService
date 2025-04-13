using QuestionService.Api;
using QuestionService.Api.Middlewares;
using QuestionService.Application.DependencyInjection;
using QuestionService.DAL.DependencyInjection;
using QuestionService.Domain.Settings;
using QuestionService.GraphQl.DependencyInjection;
using QuestionService.Grpc.DependencyInjection;
using QuestionService.Outbox.DependencyInjection;
using QuestionService.ReputationProducer.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KeycloakSettings>(builder.Configuration.GetSection(nameof(KeycloakSettings)));
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(nameof(KafkaSettings)));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));
builder.Services.Configure<BusinessRules>(builder.Configuration.GetSection(nameof(BusinessRules)));
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

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

app.UseRouting();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<WarningHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ClaimsValidationMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseGraphQl();

await builder.Services.MigrateDatabaseAsync();
app.UseForwardedHeaders(builder.Configuration);

app.LogListeningUrls();

await app.RunAsync();

public partial class Program;