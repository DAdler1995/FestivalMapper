using FestivalMapper.App.Interfaces;
using FestivalMapper.App.Models;
using FestivalMapper.App.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

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

        // image related
        private const long MaxUploadBytes = 20 * 1024 * 1024; // 20MB
        private static readonly string[] AllowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        private string? _mapError;

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

        private void Cancel()
        {
            if (_form is null)
            {
                NavManager.NavigateTo("/");
                return;
            }

            NavManager.NavigateTo($"/festival/{_form.Id}");
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

        private async Task OnMapSelected(InputFileChangeEventArgs e)
        {
            _mapError = null;

            var file = e.File;
            if (file is null || _form is null)
            {
                return;
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                _mapError = $"Unsupported image type: {file.ContentType}. Use JPEG, PNG, GIF, or WebP.";
                return;
            }

            if (file.Size > MaxUploadBytes)
            {
                _mapError = $"Image file is too large ({file.Size / 1024 / 1024} MB). Max {MaxUploadBytes / 1024 / 1024} MB.";
                return;
            }

            // Read entire file to memory and convert to base64
            using var read = file.OpenReadStream(MaxUploadBytes);
            using var ms = new MemoryStream();
            await read.CopyToAsync(ms);
            var bytes = ms.ToArray();

            // todo: downscale/compress image before converting to base64

            _form.MapImageBase64 = Convert.ToBase64String(bytes);
            _form.MapImageContentType = file.ContentType;
        }

        private void RemoveMap()
        {
            _mapError = null;
            if (_form is null) return;
            _form.MapImageBase64 = null;
            _form.MapImageContentType = null;
        }

        private static string BuildDataUrl(string contentType, string base64)
        {
            return $"data:{contentType};base64,{base64}";
        }

        private FestivalModel MapToDomain(FestivalFormViewModel form)
        {
            return new FestivalModel(
                form.Id, 
                form.Name, 
                form.StartDate, 
                form.EndDate, 
                form.City, 
                form.State, 
                TimeZoneInfo.Local.Id,
                form.MapImageBase64,
                form.MapImageContentType, 
                []);
        }

        private FestivalFormViewModel? MapToViewModl(FestivalModel festival)
        {
            return new FestivalFormViewModel
            {
                Id = festival.Id,
                Name = festival.Name,
                StartDate = festival.StartDate,
                EndDate = festival.EndDate,
                City = festival.City,
                State = festival.State,
                MapImageBase64 = festival.MapImageBase64,
                MapImageContentType = festival.MapImageContentType
            };
        }
    }
}