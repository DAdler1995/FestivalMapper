using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalMapper.App.Models.ViewModels
{
    public record class FestivalListItem(
        Guid Id, 
        string Name, 
        string DateRange,
        string? CityState = null
    );
}
