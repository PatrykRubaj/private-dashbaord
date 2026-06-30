using System.ComponentModel.DataAnnotations;

namespace Core.Model.DTO;

public class SleepEntryRangeDto : IValidatableObject
{
    [Required] public DateOnly? From { get; set; }
    [Required] public DateOnly? Until { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (From > Until)
        {
            yield return new ValidationResult(
                "From must be less than or equal to Until.",
                new[] { nameof(From), nameof(Until) });
        }
    }
}
