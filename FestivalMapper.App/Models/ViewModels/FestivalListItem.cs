using System.Text.Json.Serialization;

namespace FestivalMapper.App.Models.ViewModels
{
    public record class FestivalListItem(
        Guid Id, 
        string Name,
        DateOnly StartDate,
        DateOnly EndDate,
        string? City,
        string? State
    )
    {
        [JsonIgnore]
        public string DateRange
        {
            get
            {
                // If Start and End are in different years, show both with years
                if (StartDate.Year != EndDate.Year)
                    return $"{StartDate:MMM d, yyyy} - {EndDate:MMM d, yyyy}";

                // Same year: only show year if it's not the current year
                var currentYear = DateOnly.FromDateTime(DateTime.Today).Year;
                var includeYear = StartDate.Year != currentYear;

                return includeYear
                    ? $"{StartDate:MMM d} - {EndDate:MMM d} {StartDate:yyyy}"
                    : $"{StartDate:MMM d} - {EndDate:MMM d}";
            }
        }

        [JsonIgnore]
        public string? CityStateString
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(City) && !string.IsNullOrWhiteSpace(State))
                    return $"{City}, {State}";
                if (!string.IsNullOrWhiteSpace(City))
                    return City;
                if (!string.IsNullOrWhiteSpace(State))
                    return State;
                return null;
            }
        }
    };
}
