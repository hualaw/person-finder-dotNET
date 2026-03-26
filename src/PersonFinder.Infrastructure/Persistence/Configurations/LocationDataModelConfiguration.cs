using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonFinder.Infrastructure.Persistence.Models;

namespace PersonFinder.Infrastructure.Persistence.Configurations;

public sealed class LocationDataModelConfiguration : IEntityTypeConfiguration<LocationDataModel>
{
    public void Configure(EntityTypeBuilder<LocationDataModel> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.PersonId)
            .HasColumnName("person_id")
            .IsRequired();

        builder.Property(x => x.Latitude)
            .HasColumnName("latitude")
            .IsRequired();

        builder.Property(x => x.Longitude)
            .HasColumnName("longitude")
            .IsRequired();

        builder.HasIndex(x => x.PersonId)
            .IsUnique();
    }
}
