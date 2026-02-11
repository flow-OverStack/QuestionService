using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Settings;

namespace QuestionService.DAL.Configurations;

public class ViewConfiguration : IEntityTypeConfiguration<View>
{
    private const int IPv6MaxLength = 39;

    public void Configure(EntityTypeBuilder<View> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.QuestionId).IsRequired();
        builder.Property(x => x.UserId).IsRequired(false);
        builder.Property(x => x.UserIp).HasMaxLength(IPv6MaxLength).IsFixedLength().IsRequired(false);
        builder.Property(x => x.UserFingerprint).IsRequired(false).HasMaxLength(EntityConstraints.UserFingerprintLength)
            .IsFixedLength();
        builder.HasQueryFilter(x => x.Question.Enabled);

        builder.ToTable(t => t.HasCheckConstraint("CK_View_UserId_Or_UserIpAndFingerprint", """
            "UserId" IS NOT NULL OR ("UserFingerprint" IS NOT NULL AND "UserIp" IS NOT NULL)
            """));

        builder.HasIndex(x => new { x.QuestionId, x.UserId, x.UserIp, x.UserFingerprint }).IsUnique();
    }
}