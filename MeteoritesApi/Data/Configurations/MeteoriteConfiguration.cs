using MeteoritesApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeteoritesApi.Data.Configurations;

public class MeteoriteConfiguration : IEntityTypeConfiguration<Meteorite>
{
    public void Configure(EntityTypeBuilder<Meteorite> builder)
    {
        builder.ToTable("meteorites");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.NameType)
            .HasColumnName("name_type")
            .HasMaxLength(50);

        builder.Property(x => x.Recclass)
            .HasColumnName("recclass")
            .HasMaxLength(100);

        builder.Property(x => x.MassGram)
            .HasColumnName("mass_gram")
            .HasPrecision(18, 2);

        builder.Property(x => x.Year)
            .HasColumnName("year");

        builder.Property(x => x.Fall)
            .HasColumnName("fall")
            .HasMaxLength(20);

        builder.Property(x => x.Latitude)
            .HasColumnName("latitude");

        builder.Property(x => x.Longitude)
            .HasColumnName("longitude");

        builder.Property(x => x.GeoLocationType)
            .HasColumnName("geolocation_type")
            .HasMaxLength(50);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(x => x.Year).HasDatabaseName("ix_meteorites_year");
        builder.HasIndex(x => x.Recclass).HasDatabaseName("ix_meteorites_recclass");
        builder.HasIndex(x => new { x.Year, x.Recclass }).HasDatabaseName("ix_meteorites_year_recclass");
    }
}

