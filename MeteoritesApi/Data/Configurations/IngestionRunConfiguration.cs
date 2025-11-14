using MeteoritesApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeteoritesApi.Data.Configurations;

public class IngestionRunConfiguration : IEntityTypeConfiguration<IngestionRun>
{
    public void Configure(EntityTypeBuilder<IngestionRun> builder)
    {
        builder.ToTable("ingestion_runs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.HangfireJobId).HasColumnName("hangfire_job_id").HasMaxLength(100);
        builder.Property(x => x.StartedAt).HasColumnName("started_at");
        builder.Property(x => x.FinishedAt).HasColumnName("finished_at");
        builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
        builder.Property(x => x.Inserted).HasColumnName("inserted");
        builder.Property(x => x.Updated).HasColumnName("updated");
        builder.Property(x => x.Deleted).HasColumnName("deleted");
        builder.Property(x => x.ErrorMessage).HasColumnName("error_message").HasMaxLength(2000);

        builder.HasIndex(x => x.StartedAt).HasDatabaseName("ix_ingestion_runs_started_at");
    }
}

