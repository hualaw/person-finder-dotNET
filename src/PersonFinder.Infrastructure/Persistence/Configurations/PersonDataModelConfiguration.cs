using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonFinder.Infrastructure.Persistence.Models;

namespace PersonFinder.Infrastructure.Persistence.Configurations;

public sealed class PersonDataModelConfiguration : IEntityTypeConfiguration<PersonDataModel>
{
    public void Configure(EntityTypeBuilder<PersonDataModel> builder)
    {
        builder.ToTable("persons");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.JobTitle)
            .HasColumnName("job_title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Hobbies)
            .HasColumnName("hobbies")
            .HasColumnType("text[]")
            .IsRequired();

        builder.Property(x => x.Bio)
            .HasColumnName("bio")
            .IsRequired();
    }
}
