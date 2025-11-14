using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeteoritesApi.Entities;

public class Meteorite
{
    [Key]
    [MaxLength(50)]
    public string Id { get; set; } = default!;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = default!;

    [MaxLength(50)]
    public string? NameType { get; set; }

    [MaxLength(100)]
    public string? Recclass { get; set; }

    public decimal? MassGram { get; set; }

    public int? Year { get; set; }

    [MaxLength(20)]
    public string? Fall { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    [MaxLength(50)]
    public string? GeoLocationType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

