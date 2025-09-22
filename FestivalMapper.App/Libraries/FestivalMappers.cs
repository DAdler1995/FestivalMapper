using FestivalMapper.App.Models;
using FestivalMapper.App.Models.ViewModels;

namespace FestivalMapper.App.Libraries
{
    public static class FestivalMappers
    {
        public static FestivalListItem ToListItemViewModel(FestivalModel f)
        {
            return new FestivalListItem(f.Id, f.Name, f.StartDate, f.EndDate, f.City, f.State);
        }
    }
}
