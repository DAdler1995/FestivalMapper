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
        Task<IReadOnlyList<FestivalModel>> GetAllAsync(CancellationToken ct = default);
        Task<FestivalModel?> GetByIdASync(Guid Id, CancellationToken ct = default);
        Task SaveAsync(FestivalModel festival, CancellationToken ct = default);
        Task DeleteAsync(Guid id,  CancellationToken ct = default);
    }
}
