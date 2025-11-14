namespace MeteoritesApi.Dtos;

public class MeteoriteSummaryQuery
{
    public int? YearFrom { get; set; }

    public int? YearTo { get; set; }

    public string? Recclass { get; set; }

    public string? NamePart { get; set; }

    public string SortField { get; set; } = "year";

    public string SortOrder { get; set; } = "asc";

    public string NormalizeSortField()
        => SortField?.Trim().ToLowerInvariant() switch
        {
            "year" => "year",
            "count" => "count",
            "mass" => "mass",
            _ => "year"
        };

    public bool IsSortDescending()
        => SortOrder?.Trim().Equals("desc", StringComparison.OrdinalIgnoreCase) == true;
}

