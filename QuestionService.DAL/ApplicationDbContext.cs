using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuestionService.DAL.Interceptors;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Settings;
using ILogger = Serilog.ILogger;

namespace QuestionService.DAL;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ILogger logger,
    IOptions<BusinessRules> businessRules)
    : DbContext(options)
{
    private readonly BusinessRules _businessRules = businessRules.Value;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(logger.Information, LogLevel.Information);
        optionsBuilder.AddInterceptors(new DateInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        ApplyTagRules(modelBuilder);
        ApplyViewRules(modelBuilder);
    }

    private void ApplyTagRules(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>()
            .Property(x => x.Name).IsRequired().HasMaxLength(_businessRules.TagMaxLength);

        modelBuilder.Entity<Tag>()
            .Property(x => x.Description).IsRequired().HasMaxLength(_businessRules.TagDescriptionMaxLength);
    }

    private void ApplyViewRules(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<View>()
            .Property(x => x.UserFingerprint).HasMaxLength(_businessRules.UserFingerprintLength).IsRequired(false);
    }
}