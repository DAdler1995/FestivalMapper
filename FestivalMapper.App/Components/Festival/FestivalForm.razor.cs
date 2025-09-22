using FestivalMapper.App.Interfaces;
using FestivalMapper.App.Models;
using FestivalMapper.App.Models.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FestivalMapper.App.Components.Festival
{
    public partial class FestivalForm
    {
        [Parameter] public Guid? Id { get; set; }

        [Inject] private IFestivalService Service { get; set; } = default!;
        [Inject] private NavigationManager NavManager { get; set; } = default!;

        private FestivalFormViewModel? _form;
        private bool IsNew => Id is null;

        private static DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

        protected override async Task OnParametersSetAsync()
        {
            if (IsNew)
            {
                _form = new FestivalFormViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = string.Empty,
                    StartDate = Today,
                    EndDate = Today,
                    City = null,
                    State = null
                };

                return;
            }

            var festival = await Service.GetFestivalAsync(Id!.Value);
            if (festival is null)
            {
                // festival not found, return home
                NavManager.NavigateTo("/");
                return;
            }

            _form = MapToViewModl(festival);
        }

        private async Task SaveAsync()
        {
            if (_form is null)
            {
                return;
            }

            if (_form.StartDate > _form.EndDate)
            {
                return;
            }

            var model = MapToDomain(_form);
            await Service.SaveFestivalAsync(model);

            NavManager.NavigateTo("/");
        }

        private async Task DeleteAsync()
        {
            if (IsNew || Id is null)
            {
                // festival is null or not found, just return home
                NavManager.NavigateTo("/");
            }

            await Service.DeleteFestivalAsync(Id.Value);
            NavManager.NavigateTo("/");
        }

        private FestivalModel MapToDomain(FestivalFormViewModel form)
        {
            return new FestivalModel(form.Id, form.Name, form.StartDate, form.EndDate, form.City, form.State, null, "", []);
        }

        private FestivalFormViewModel? MapToViewModl(Models.FestivalModel festival)
        {
            return new FestivalFormViewModel
            {
                Id = festival.Id,
                Name = festival.Name,
                StartDate = festival.StartDate,
                EndDate = festival.EndDate,
                City = festival.City,
                State = festival.State
            };
        }
    }
}