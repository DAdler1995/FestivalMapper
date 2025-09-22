using FestivalMapper.App.Interfaces;
using FestivalMapper.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FestivalMapper.App.Infrastructure
{
    public sealed class JsonFestivalRepositroy : IFestivalRepository
    {
        private readonly string _root;
        private readonly JsonSerializerOptions _json;

        public JsonFestivalRepositroy(string root, JsonSerializerOptions? json = null)
        {
            _root = root;
            Directory.CreateDirectory(_root); // ensure it exists

            // use provided options otherwise setup default options
            _json = json ?? new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // helper for getting a specific festival path
        private string PathFor(Guid Id) => Path.Combine(_root, $"{Id}.festival.json");


        public async Task<IReadOnlyList<FestivalModel>> GetAllAsync(CancellationToken ct = default)
        {
            var results = new List<FestivalModel>();
            foreach (var file in Directory.EnumerateFiles(_root, "*.festival.json"))
            {
                await using var stream = File.OpenRead(file);
                var festival = await JsonSerializer.DeserializeAsync<FestivalModel>(stream, _json, ct);
                if (festival is not null)
                {
                    results.Add(festival);
                }
            }

            return results;
        }

        public async Task<FestivalModel?> GetByIdASync(Guid Id, CancellationToken ct = default)
        {
            var path = PathFor(Id);
            
            if (!File.Exists(path))
            {
                return null;
            }

            await using var stream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<FestivalModel>(stream, _json, ct);
        }

        public async Task SaveAsync(FestivalModel festival, CancellationToken ct = default)
        {
            // get the path for the festival - create a new guid if needed
            var path = PathFor(festival.Id == Guid.Empty ? Guid.NewGuid() : festival.Id);
            var stable = festival with { Id = festival.Id == Guid.Empty ? Guid.Parse(Path.GetFileNameWithoutExtension(path)) : festival.Id };

            var tmp = path + ".tmp";
            await using ( var stream = File.Create(tmp))
            {
                await JsonSerializer.SerializeAsync(stream, stable, _json, ct);
            }
            File.Move(tmp, path, true);
        }

        public Task DeleteAsync(Guid Id, CancellationToken ct = default)
        {
            var path = PathFor(Id);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return Task.CompletedTask;
        }
    }
}
