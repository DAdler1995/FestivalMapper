using FestivalMapper.App.Models;
using FestivalMapper.App.Models.ViewModels;

namespace FestivalMapper.App.Libraries
{
    public static class FestivalMappers
    {
        public static FestivalListItem ToListItemViewModel(Festival f)
        {
            var sameYearAsToday = f.StartDate.Year == DateOnly.FromDateTime(DateTime.Today).Year;
            var dateRange = $"{f.StartDate:MMM d} - {f.EndDate:MMM d}{(sameYearAsToday ? "" : $" {f.StartDate:yyyy}")}";
            var cityState = string.IsNullOrWhiteSpace(f.City) ? null : $"{f.City}, {f.State}";

            return new FestivalListItem(f.Id, f.Name, dateRange, cityState);
        }
    }
}
