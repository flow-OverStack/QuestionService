using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestionService.Domain.Entities;

namespace QuestionService.DAL.Configurations;

public class ViewConfiguration : IEntityTypeConfiguration<View>
{
    public void Configure(EntityTypeBuilder<View> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.QuestionId).IsRequired();
        builder.Property(x => x.UserId).IsRequired(false);
        builder.Property(x => x.UserIp).IsRequired(false);
        builder.Property(x => x.UserFingerprint).IsRequired(false);

        builder.ToTable(t => t.HasCheckConstraint("CK_View_UserId_Or_UserIpAndFingerprint", """
            (UserId IS NOT NULL OR (UserFingerprint IS NOT NULL AND UserIp IS NOT NULL))
            """));
    }
}