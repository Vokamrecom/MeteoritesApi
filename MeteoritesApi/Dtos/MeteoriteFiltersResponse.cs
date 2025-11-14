namespace MeteoritesApi.Dtos;

public class MeteoriteFiltersResponse
{
    public int MinYear { get; set; }

    public int MaxYear { get; set; }

    public IReadOnlyCollection<string> Recclasses { get; set; } = Array.Empty<string>();
}

