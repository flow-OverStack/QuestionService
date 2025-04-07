using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestionService.Domain.Entities;

namespace QuestionService.DAL.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.Body).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.LastModifiedAt);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Reputation).IsRequired().HasDefaultValue(0);

        builder.HasMany(x => x.Tags)
            .WithMany(x => x.Questions)
            .UsingEntity<QuestionTag>(x => x.HasOne<Tag>().WithMany().HasForeignKey(y => y.TagName),
                x => x.HasOne<Question>().WithMany().HasForeignKey(y => y.QuestionId));

        builder.HasMany(x => x.Votes)
            .WithOne(x => x.Question)
            .HasForeignKey(x => x.QuestionId)
            .HasPrincipalKey(x => x.Id);

        builder.HasMany(x => x.Views)
            .WithOne(x => x.Question)
            .HasForeignKey(x => x.QuestionId)
            .HasPrincipalKey(x => x.Id);
    }
}