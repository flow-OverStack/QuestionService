using QuestionService.Api;
using QuestionService.Api.Middlewares;
using QuestionService.Application.DependencyInjection;
using QuestionService.DAL.DependencyInjection;
using QuestionService.Domain.Settings;
using QuestionService.GraphQlClient.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KeycloakSettings>(builder.Configuration.GetSection(nameof(KeycloakSettings)));
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(nameof(KafkaSettings)));

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

builder.Services.AddAuthenticationAndAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddGraphQlClient();
builder.Services.AddApplication();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<WarningHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

await builder.Services.MigrateDatabaseAsync();

app.LogListeningUrls();

await app.RunAsync();