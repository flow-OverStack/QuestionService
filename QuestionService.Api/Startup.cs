using System.Net;
using System.Reflection;
using Asp.Versioning;
using Confluent.Kafka;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using QuestionService.DAL;
using QuestionService.Domain.Settings;
using Serilog;
using Path = System.IO.Path;

namespace QuestionService.Api;

/// <summary>
///     A utility class that provides extension methods for configuring and setting up the application's services and middleware.
/// </summary>
public static class Startup
{
    private const string AppStartupSectionName = "AppStartupSettings";
    private const string TelemetrySectionName = "TelemetrySettings";
    private const string AspireDashboardUrlName = "AspireDashboardUrl";
    private const string JaegerUrlName = "JaegerUrl";
    private const string LogstashUrlName = "LogstashUrl";
    private const string AspireDashboardHealthCheckUrlName = "AspireDashboardHealthCheckUrl";
    private const string JaegerHealthCheckUrlName = "JaegerHealthCheckUrl";
    private const string PrometheusUrlName = "PrometheusUrl";
    private const string UserServiceHealthCheckUrlName = "UserServiceHealthCheckUrl";
    private const string ElasticSearchUrlName = "ElasticSearchUrl";
    private const string AppStartupUrlLogName = "AppStartupUrlLog";
    private const string ServiceName = "QuestionService";

    /// <summary>
    ///     Configures JWT Bearer authentication and authorization services for the application.
    /// </summary>
    /// <param name="services">The service collection to which authentication and authorization services are added.</param>
    public static void AddAuthenticationAndAuthorization(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var keycloakSettings =
                services.BuildServiceProvider().GetRequiredService<IOptions<KeycloakSettings>>().Value;

            options.MapInboundClaims = false; // For userId because sub mapped into the same ClaimsType
            options.RequireHttpsMetadata = false;
            options.MetadataAddress = keycloakSettings.MetadataAddress;
            options.Audience = keycloakSettings.Audience;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });
        services.AddAuthorization();
    }

    /// <summary>
    ///     Configures and adds Swagger documentation generation with JWT authentication support to the service collection.
    ///     Includes API versioning, security definitions, and XML documentation.
    /// </summary>
    /// <param name="services">The service collection to which Swagger services are added.</param>
    public static void AddSwagger(this IServiceCollection services)
    {
        const string apiVersion = "v1";

        services.AddApiVersioning()
            .AddApiExplorer(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
            });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(apiVersion, new OpenApiInfo
            {
                Version = "v1",
                Title = "QuestionService.Api",
                Description = "QuestionService api v1",
                //maybe add in future
                //TermsOfService = termsOfServiceUrl,
                Contact = new OpenApiContact
                {
                    Name = "UserService api contact"
                    //maybe add in future
                    //Url = termsOfServiceUrl
                }
            });

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please write valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header
                    },
                    []
                }
            });

            var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
        });
    }

    /// <summary>Logs all URLs on which the application is listening when it starts.</summary>
    /// <param name="app">The web application to which the middleware is added.</param>
    public static void LogListeningUrls(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var hosts = app.GetHosts().ToList();

            var appStartupHostLog =
                app.Configuration.GetSection(AppStartupSectionName).GetValue<string>(AppStartupUrlLogName);

            hosts.ForEach(host => Log.Information("{0}{1}", appStartupHostLog, host));
        });
    }

    /// <summary>
    ///     Configures the application to use X-Forwarded-For headers from known proxy servers
    /// </summary>
    /// <param name="app">The WebApplication instance to configure forwarded headers for</param>
    /// <param name="configuration">The configuration containing known proxy addresses</param>
    public static void UseForwardedHeaders(this WebApplication app, IConfiguration configuration)
    {
        var knownProxiesString =
            configuration.GetSection(AppStartupSectionName).GetSection("KnownProxies").Get<string[]>();
        if (knownProxiesString == null || knownProxiesString.Length == 0) return;

        var options = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor
        };
        knownProxiesString.ToList().ForEach(x => options.KnownProxies.Add(IPAddress.Parse(x)));

        app.UseForwardedHeaders(options);
    }

    /// <summary>
    ///     Configures Hangfire with PostgreSQL storage and adds it to the service collection.
    ///     Sets up job retry policies, serialization settings, and logging integration.
    /// </summary>
    /// <param name="services">The service collection to which Hangfire services are added.</param>
    /// <param name="configuration">The configuration containing database connection settings.</param>
    public static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        const string postgresSqlConnectionName = "PostgresSQL";

        services.AddHangfire(x => x.UsePostgreSqlStorage(options =>
            {
                var connectionString = configuration.GetConnectionString(postgresSqlConnectionName);
                options.UseNpgsqlConnection(connectionString);
            })
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSerilogLogProvider()
            .UseFilter(new AutomaticRetryAttribute
            {
                Attempts = 10,
                DelaysInSeconds = [30, 60, 300, 600, 1800, 43200, 86400] //30sec, 1min, 5min, 10min, 1h, 12h, 24h
            }));

        services.AddHangfireServer();
    }

    /// <summary>
    ///     Enables the Hangfire dashboard in development environments.
    ///     This provides a web interface for monitoring and managing background jobs.
    /// </summary>
    /// <param name="app">The web application to which Hangfire middleware is added.</param>
    public static void UseHangfire(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseHangfireDashboard();
    }

    /// <summary>
    ///     Configures and adds OpenTelemetry for observability, including metrics, tracing, and logging instrumentation.
    /// </summary>
    /// <param name="builder">The instance of <see cref="WebApplicationBuilder" /> being configured.</param>
    public static void AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var telemetryConfiguration =
            builder.Configuration.GetSection(AppStartupSectionName).GetSection(TelemetrySectionName);

        var aspireDashboardUri = new Uri(telemetryConfiguration.GetValue<string>(AspireDashboardUrlName)!);
        var jaegerUri = new Uri(telemetryConfiguration.GetValue<string>(JaegerUrlName)!);

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(res => res.AddService(ServiceName))
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                metrics.AddOtlpExporter(options => options.Endpoint = aspireDashboardUri).AddPrometheusExporter();
            })
            .WithTracing(traces =>
            {
                traces.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                traces.AddOtlpExporter(options => options.Endpoint = aspireDashboardUri)
                    .AddOtlpExporter(options => options.Endpoint = jaegerUri);
            });
    }

    /// <summary>
    ///     Configures structured logging for the application using Serilog with OpenTelemetry integration.
    /// </summary>
    /// <param name="host">The host builder used to configure the application.</param>
    /// <param name="appConfiguration">The application configuration from which logging settings are retrieved.</param>
    public static void AddLogging(this IHostBuilder host, IConfiguration appConfiguration)
    {
        const string serviceNameKey = "service.name";
        const string serviceInstanceIdKey = "service.instance.id";

        host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration).WriteTo
            .OpenTelemetry(options =>
            {
                options.Endpoint = appConfiguration.GetSection(AppStartupSectionName)
                    .GetSection(TelemetrySectionName).GetValue<string>(AspireDashboardUrlName);
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    [serviceNameKey] = ServiceName,
                    [serviceInstanceIdKey] = Guid.NewGuid()
                };
            }));
    }

    /// <summary>
    ///     Adds health checks to the application, including checks for database context, Kafka, Redis, Elasticsearch,
    ///     and Hangfire with specific configurations.
    /// </summary>
    /// <param name="services">The service collection to which health check services are added.</param>
    /// <param name="configuration">The application configuration used to retrieve settings for health checks.</param>
    public static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaHost = configuration.GetSection(nameof(KafkaSettings)).GetValue<string>(nameof(KafkaSettings.Host));
        var redisSettings = configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>()!;
        var keycloakSettings = configuration.GetSection(nameof(KeycloakSettings)).Get<KeycloakSettings>()!;
        var redisConnectionString = $"{redisSettings.Host}:{redisSettings.Port},password={redisSettings.Password}";

        var telemetrySection = configuration.GetSection(AppStartupSectionName).GetSection(TelemetrySectionName);
        var elasticSearchUrl = telemetrySection.GetValue<string>(ElasticSearchUrlName)!;
        var logstashUrl = telemetrySection.GetValue<string>(LogstashUrlName)!;
        var prometheusUrl = telemetrySection.GetValue<string>(PrometheusUrlName)!;
        var jaegerUrl = telemetrySection.GetValue<string>(JaegerHealthCheckUrlName)!;
        var aspireDashboardUrl = telemetrySection.GetValue<string>(AspireDashboardHealthCheckUrlName)!;
        var userServiceHealthCheckUrl = telemetrySection.GetValue<string>(UserServiceHealthCheckUrlName)!;

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>()
            .AddKafka(new ProducerConfig { BootstrapServers = kafkaHost })
            .AddRedis(redisConnectionString)
            .AddElasticsearch(elasticSearchUrl)
            .AddHangfire(options =>
            {
                options.MinimumAvailableServers = 1;
                options.MaximumJobsFailed = 10; // 10 failed jobs means the server is down
            })
            .AddUrlGroup(new Uri(prometheusUrl), name: "prometheus")
            .AddUrlGroup(new Uri(logstashUrl), name: "logstash")
            .AddUrlGroup(new Uri(keycloakSettings.Host), name: "keycloak")
            .AddUrlGroup(new Uri(jaegerUrl), name: "jaeger")
            .AddUrlGroup(new Uri(aspireDashboardUrl), name: "aspire")
            .AddUrlGroup(new Uri(userServiceHealthCheckUrl), name: "user-service");
    }

    private static IEnumerable<string> GetHosts(this WebApplication app)
    {
        HashSet<string> hosts = [];

        var serverAddressesFeature = ((IApplicationBuilder)app).ServerFeatures.Get<IServerAddressesFeature>();
        serverAddressesFeature?.Addresses.ToList().ForEach(x => hosts.Add(x));

        return hosts;
    }
}