using MeteoritesApi.Dtos;
using MeteoritesApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeteoritesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeteoritesController : ControllerBase
{
    private readonly IMeteoriteSummaryService _summaryService;
    private readonly IMeteoriteFilterService _filterService;

    public MeteoritesController(
        IMeteoriteSummaryService summaryService,
        IMeteoriteFilterService filterService)
    {
        _summaryService = summaryService;
        _filterService = filterService;
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(MeteoriteSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MeteoriteSummaryResponse>> GetSummary([FromQuery] MeteoriteSummaryQuery query, CancellationToken cancellationToken)
    {
        var response = await _summaryService.GetSummaryAsync(query, cancellationToken);
        return Ok(response);
    }

    [HttpGet("filters")]
    [ProducesResponseType(typeof(MeteoriteFiltersResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<MeteoriteFiltersResponse>> GetFilters(CancellationToken cancellationToken)
    {
        var response = await _filterService.GetFiltersAsync(cancellationToken);
        return Ok(response);
    }
}

