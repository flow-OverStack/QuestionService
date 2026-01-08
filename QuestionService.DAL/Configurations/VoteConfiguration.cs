using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestionService.Domain.Entities;

namespace QuestionService.DAL.Configurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.QuestionId).IsRequired();
        builder.HasQueryFilter(x => x.Question.Enabled);

        builder.HasKey(x => new { x.QuestionId, x.UserId });
    }
}