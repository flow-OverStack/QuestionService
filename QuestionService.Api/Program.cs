using QuestionService.Api;
using QuestionService.Api.Middlewares;
using QuestionService.Application.DependencyInjection;
using QuestionService.DAL.DependencyInjection;
using QuestionService.Domain.Settings;
using QuestionService.Grpc.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KeycloakSettings>(builder.Configuration.GetSection(nameof(KeycloakSettings)));
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(nameof(KafkaSettings)));
builder.Services.Configure<BusinessRules>(builder.Configuration.GetSection(nameof(BusinessRules)));

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

builder.Services.AddAuthenticationAndAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddGrpcClients();

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

app.UseHttpsRedirection();
app.MapControllers();

await builder.Services.MigrateDatabaseAsync();

app.LogListeningUrls();

await app.RunAsync();