using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalMapper.App.Services
{
    public class ImagePickerService
    {
        public async Task<string?> PickImageAsBase64Async()
        {
			try
			{
				var file = await FilePicker.Default.PickAsync(new PickOptions
				{
					PickerTitle = "Select Festival Map Iamge",
					FileTypes = FilePickerFileType.Images
				});

				if (file == null)
				{
					throw new Exception("Invalid Image Seclected");
                }

				using var Stream = await file.OpenReadAsync();
				using var ms = new MemoryStream();

				await Stream.CopyToAsync(ms);
				var bytes = ms.ToArray();

				return Convert.ToBase64String(bytes);

			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error picking image: {ex.Message}");
            }

            return null;
        }
    }
}
