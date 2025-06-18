using FestivalMapper.App.Models;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace FestivalMapper.App.Services
{
    public class MapStorageService
    {
        private readonly string SaveDirectory;

        public MapStorageService()
        {
            SaveDirectory = Path.Combine(FileSystem.AppDataDirectory, "maps");
            Directory.CreateDirectory(SaveDirectory);
        }

        public async Task SaveMapAsync(FestivalMap map)
        {
            // save date as UTC
            map.FestivalStartDate = map.FestivalStartDate.Date.ToUniversalTime();

            var fileNameSafe = string.Join("_", map.Id.Split(Path.GetInvalidFileNameChars()));
            var filePath = Path.Combine(SaveDirectory, $"{fileNameSafe}.festivalmap");

            var json = JsonSerializer.Serialize(map, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<List<FestivalMap>> LoadAllMapsAsync()
        {
            var files = Directory.GetFiles(SaveDirectory, "*.festivalmap");
            var maps = new List<FestivalMap>();

            foreach (var file in files)
            {
                var json = await File.ReadAllTextAsync(file);
                try
                {
                    var map = JsonSerializer.Deserialize<FestivalMap>(json);
                    if (map != null)
                    {
                        maps.Add(map);
                    }
                }
                catch
                {
                    // do nothing with invalid maps currently
                }

            }

            return maps;
        }

        public async Task<FestivalMap?> GetFestivalMapAsync(string fileName)
        {
            var fileNameSafe = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
            var filePath = Path.Combine(SaveDirectory, $"{fileNameSafe}.festivalmap");

            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                try
                {
                    var map = JsonSerializer.Deserialize<FestivalMap>(json);
                    return map;
                }
                catch
                {
                    // do nothing with invalid maps currently
                }
            }

            return null;

        }

        public async Task DeleteMapAsync(string festivalId)
        {
            var fileNameSafe = string.Join("_", festivalId.Split(Path.GetInvalidFileNameChars()));
            var filePath = Path.Combine(SaveDirectory, $"{fileNameSafe}.festivalmap");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task<FestivalMap?> ImportMapAsync(FileResult file)
        {
            try
            {
                if (file == null || Path.GetExtension(file.FileName)?.ToLower() != ".festivalmap")
                {
                    return null;
                }

                using var stream = await file.OpenReadAsync();
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                var map = JsonSerializer.Deserialize<FestivalMap>(json);
                if (map == null)
                {
                    return null;
                }

                var fileNameSafe = string.Join("_", map.Id.Split(Path.GetInvalidFileNameChars()));
                var filePath = Path.Combine(SaveDirectory, $"{fileNameSafe}.festivalmap");

                await File.WriteAllTextAsync(filePath, json);
                return map;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task ShareMap(string festivalId)
        {
            var fileNameSafe = string.Join("_", festivalId.Split(Path.GetInvalidFileNameChars()));
            var filePath = Path.Combine(SaveDirectory, $"{fileNameSafe}.festivalmap");

            if (File.Exists(filePath))
            {
                var festivalMap = "";
                var json = await File.ReadAllTextAsync(filePath);
                try
                {
                    var map = JsonSerializer.Deserialize<FestivalMap>(json);
                    await Share.Default.RequestAsync(new ShareFileRequest
                    {
                        Title = $"Festival Map: {map.FestivalName}",
                        File = new ShareFile(filePath)
                    });
                }
                catch
                {
                    // do nothing with invalid maps currently
                }

            }
        }
    }
}
