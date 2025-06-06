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
    }
}
