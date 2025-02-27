using QuestionService.Api;
using QuestionService.Api.Middlewares;
using QuestionService.DAL.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();

builder.Services.AddAuthenticationAndAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDataAccessLayer(builder.Configuration);

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