namespace FestivalMapper.App.Libraries
{
    public static class FileTypeHelper
    {
        public static readonly FilePickerFileType Json = new(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, new[] { ".json" } },
            { DevicePlatform.macOS, new[] { "public.json" } },
            { DevicePlatform.iOS, new[] { "public.json" } },
            { DevicePlatform.Android, new[] { "application/json" } },
        });

        public static readonly FilePickerFileType FestivalMap = new(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, new[] { ".festivalmap" } }, // windows will restrict it to only ".festivalmap" extensions
            { DevicePlatform.macOS, new[] { "public.item" } },  // Show all files, rely on extension check
            { DevicePlatform.iOS, new[] { "public.item" } },    // Same as above
            { DevicePlatform.Android, new[] { "application/octet-stream" } } // All unknown binary types
        });
    }
}
