using FestivalMapper.App.Models;
using FestivalMapper.App.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalMapper.App.Interfaces
{
    public interface IFestivalService
    {
        Task<(IReadOnlyList<FestivalListItem> Upcoming, IReadOnlyList<FestivalListItem> Past)> GetFestivalListsAsync(CancellationToken ct = default);
        Task<FestivalModel?> GetFestivalAsync(Guid Id,  CancellationToken ct = default);
        Task SaveFestivalAsync(FestivalModel festival, CancellationToken ct = default);
        Task DeleteFestivalAsync(Guid Id, CancellationToken ct = default);
    }
}
