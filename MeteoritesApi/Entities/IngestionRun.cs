using System.ComponentModel.DataAnnotations;

namespace MeteoritesApi.Entities;

public class IngestionRun
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(100)]
    public string? HangfireJobId { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? FinishedAt { get; set; }

    [MaxLength(30)]
    public string Status { get; set; } = "Pending";

    public int Inserted { get; set; }

    public int Updated { get; set; }

    public int Deleted { get; set; }

    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }
}

