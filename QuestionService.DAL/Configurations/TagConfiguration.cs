using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Settings;

namespace QuestionService.DAL.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(BusinessRules.TagMaxLength);
        builder.Property(x => x.Description).HasMaxLength(BusinessRules.TagDescriptionMaxLength);

        builder.HasIndex(x => x.Name).IsUnique();
    }
}