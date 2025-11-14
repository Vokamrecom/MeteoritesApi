using FluentValidation;
using MeteoritesApi.Dtos;

namespace MeteoritesApi.Validators;

public class MeteoriteSummaryQueryValidator : AbstractValidator<MeteoriteSummaryQuery>
{
    private const int MinYear = 860;
    private static readonly int MaxYear = DateTime.UtcNow.Year + 1;

    public MeteoriteSummaryQueryValidator()
    {
        RuleFor(x => x.YearFrom)
            .GreaterThanOrEqualTo(MinYear)
            .LessThanOrEqualTo(MaxYear)
            .When(x => x.YearFrom.HasValue);

        RuleFor(x => x.YearTo)
            .GreaterThanOrEqualTo(MinYear)
            .LessThanOrEqualTo(MaxYear)
            .When(x => x.YearTo.HasValue);

        RuleFor(x => x)
            .Must(x => !x.YearFrom.HasValue || !x.YearTo.HasValue || x.YearFrom <= x.YearTo)
            .WithMessage("'YearFrom' не может быть больше 'YearTo'");

        RuleFor(x => x.Recclass)
            .MaximumLength(100);

        RuleFor(x => x.NamePart)
            .MaximumLength(100);

        RuleFor(x => x.SortField)
            .Must(value => string.IsNullOrWhiteSpace(value) ||
                           value.Equals("year", StringComparison.OrdinalIgnoreCase) ||
                           value.Equals("count", StringComparison.OrdinalIgnoreCase) ||
                           value.Equals("mass", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortField должен быть year, count или mass");

        RuleFor(x => x.SortOrder)
            .Must(value => string.IsNullOrWhiteSpace(value) ||
                           value.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                           value.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortOrder должен быть asc или desc");
    }
}

