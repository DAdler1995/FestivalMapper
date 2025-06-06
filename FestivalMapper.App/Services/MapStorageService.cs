using FestivalMapper.App.Models;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
            var fileNameSafe = string.Join("_", map.FestivalName.Split(Path.GetInvalidFileNameChars()));
            var filePath = Path.Combine(SaveDirectory, $"{fileNameSafe}.json");

            var json = JsonSerializer.Serialize(map, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<List<FestivalMap>> LoadAllMapsAsync()
        {
            var files = Directory.GetFiles(SaveDirectory, "*.json");
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

        public async Task DeleteMapAsync(string festivalName)
        {
            var fileNameSafe = string.Join("_", festivalName.Split(Path.GetInvalidFileNameChars()));
            var filePath = Path.Combine(SaveDirectory, $"{fileNameSafe}.json");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task<FestivalMap?> ImportMapAsync(FileResult file)
        {
            try
            {
                if (file == null || Path.GetExtension(file.FileName)?.ToLower() != ".json")
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

                var fileNameSafe = string.Join("_", map.FestivalName.Split(Path.GetInvalidFileNameChars()));
                var filePath = Path.Combine(SaveDirectory, $"{fileNameSafe}.json");

                await File.WriteAllTextAsync(filePath, json);
                return map;

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
