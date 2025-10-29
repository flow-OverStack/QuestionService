using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestionService.Domain.Entities;

namespace QuestionService.DAL.Configurations;

public class VoteTypeConfiguration : IEntityTypeConfiguration<VoteType>
{
    public void Configure(EntityTypeBuilder<VoteType> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired();

        builder.HasMany(x => x.Votes)
            .WithOne(x => x.VoteType)
            .HasForeignKey(x => x.VoteTypeId)
            .HasPrincipalKey(x => x.Id);
    }
}