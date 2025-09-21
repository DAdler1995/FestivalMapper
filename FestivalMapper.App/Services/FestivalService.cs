using FestivalMapper.App.Interfaces;
using FestivalMapper.App.Models;
using FestivalMapper.App.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalMapper.App.Services
{
    public sealed class FestivalService : IFestivalService
    {
        private readonly IFestivalRepository _repository;

        public FestivalService(IFestivalRepository repository)
        {
            _repository = repository;
        }
        public async Task<(IReadOnlyList<FestivalListItem> Upcoming, IReadOnlyList<FestivalListItem> Past)> GetFestivalListsAsync(CancellationToken ct = default)
        {
            var all = await _repository.GetAllAsync(ct);

            var today = DateOnly.FromDateTime(DateTime.Today);

            var festivalListItems = all.Select(f => new FestivalListItem(f.Id, f.Name, f.StartDate, f.EndDate, f.City, f.State));

            var upcoming = festivalListItems.Where(f => f.EndDate >= DateOnly.FromDateTime(DateTime.Today)).OrderBy(f => f.StartDate).ToList();
            var past = festivalListItems.Where(f => f.EndDate < DateOnly.FromDateTime(DateTime.Today)).OrderByDescending(f => f.StartDate).ToList();

            return (upcoming, past);
        }

        public async Task<Festival?> GetFestivalAsync(Guid Id, CancellationToken ct = default) => await _repository.GetByIdASync(Id, ct);

        public async Task SaveFestivalAsync(Festival festival, CancellationToken ct = default)
        {
            if (festival.StartDate > festival.EndDate)
            {
                throw new ArgumentException("StartDate must be <= EndDate");
            }

            await _repository.SaveAsync(festival, ct);
        }

        public async Task DeleteFestivalAsync(Guid Id, CancellationToken ct = default) => await _repository.DeleteAsync(Id, ct);
    }
}
