using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalMapper.App.Models.ViewModels
{
    public sealed class FestivalFormViewModel : IValidatableObject
    {
        public Guid Id { get; set; }

        [Required, StringLength(256)]
        public string Name { get; set; } = "";

        [Display(Name = "Start Date")]
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Display(Name = "End Date")]
        public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string? City { get; set; }
        public string? State { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate > EndDate)
            {
                yield return new ValidationResult(
                    "Start Date must be on or before the End Date.",
                    new[] { nameof(StartDate), nameof(EndDate) }
                    );
            }
        }

    }
}
