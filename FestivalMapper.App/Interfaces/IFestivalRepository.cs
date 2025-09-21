using FestivalMapper.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalMapper.App.Interfaces
{
    public interface IFestivalRepository
    {
        Task<IReadOnlyList<Festival>> GetAllAsync(CancellationToken ct = default);
        Task<Festival?> GetByIdASync(Guid Id, CancellationToken ct = default);
        Task SaveAsync(Festival festival, CancellationToken ct = default);
        Task DeleteAsync(Guid id,  CancellationToken ct = default);
    }
}
