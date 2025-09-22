using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalMapper.App.Models
{
    /// <summary>
    /// Root for a JSON file on disk. Lets you evolve the schema safely.
    /// </summary>
    public record class FestivalDocument(
        string SchemaVersion,
        FestivalModel Festival
    )
    {
        public static FestivalDocument New(FestivalModel festival) =>
            new("2.0.0", festival);
    }

    /// <summary>
    /// Top-level aggregate.
    /// </summary>
    public record class FestivalModel(
        Guid Id,
        string Name,
        DateOnly StartDate,
        DateOnly EndDate,
        string? City,
        string? State,
        string? TimeZoneId,          // e.g. "America/Los_Angeles"
        string? MapImageBase64,       // Base64 PNG/JPG of the festival map
        string? MapImageContentType,
        List<Stage> Stages)
    {
        public static FestivalModel New(
            string name,
            DateOnly start,
            DateOnly end,
            string? mapImageBase64 = null,
            string? MapImageContentType = null,
            string? city = null,
            string? state = null,
            string? timeZoneId = null,
            IEnumerable<Stage>? stages = null)
            => new(
                Id: Guid.NewGuid(),
                Name: name.Trim(),
                StartDate: start,
                EndDate: end,
                City: city?.Trim(),
                State: state?.Trim(),
                TimeZoneId: timeZoneId,
                MapImageBase64: mapImageBase64,
                MapImageContentType: MapImageContentType,
                Stages: (stages is null ? new List<Stage>() : stages.ToList())
            );

        /// <summary>
        /// Returns all conflicts across all stages for the entire festival.
        /// </summary>
        public IReadOnlyList<ArtistConflict> GetConflicts(bool includeCrossStage = true)
        {
            var list = new List<ArtistConflict>();

            foreach (var stage in Stages)
            {
                list.AddRange(ConflictDetection.FindConflicts(stage));
            }

            if (includeCrossStage)
            {
                for (int i = 0; i < Stages.Count; i++)
                {
                    for (int j = i + 1; j < Stages.Count; j++)
                    {
                        list.AddRange(ConflictDetection.FindCrossStageConflicts(Stages[i], Stages[j]));
                    }
                }
            }

            return list;
        }
    }

    /// <summary>
    /// A point normalized to the map image (0..1 for both X and Y).
    /// </summary>
    public readonly record struct NormalizedPoint(double X, double Y)
    {
        public static NormalizedPoint FromPixels(double xPx, double yPx, double imgWidthPx, double imgHeightPx)
            => new(xPx / imgWidthPx, yPx / imgHeightPx);
    }

    /// <summary>
    /// Hex color like "#RRGGBB" or "#RRGGBBAA".
    /// </summary>
    public readonly record struct HexColor(string Value)
    {
        public override string ToString() => Value;
        public static HexColor FromRgb(byte r, byte g, byte b) => new($"#{r:X2}{g:X2}{b:X2}");
        public static HexColor FromRgba(byte r, byte g, byte b, byte a) => new($"#{r:X2}{g:X2}{b:X2}{a:X2}");
    }

    /// <summary>
    /// Stage on the map with a location and a color tag.
    /// </summary>
    public record class Stage(
        Guid Id,
        string Name,
        NormalizedPoint Location,
        HexColor Color,
        List<Artist> Artists)
    {
        public static Stage New(
            string name,
            NormalizedPoint location,
            HexColor color,
            IEnumerable<Artist>? artists = null)
            => new(
                Id: Guid.NewGuid(),
                Name: name.Trim(),
                Location: location,
                Color: color,
                Artists: (artists is null ? new List<Artist>() : artists.ToList())
            );
    }

    /// <summary>
    /// An artist plays on a specific festival day between Start and End times (local to festival).
    /// </summary>
    public record class Artist(
        Guid Id,
        string Name,
        DateOnly PerformanceDate,
        TimeOnly Start,
        TimeOnly End)
    {
        public static Artist New(string name, DateOnly date, TimeOnly start, TimeOnly? end = null)
        {
            var actualEnd = end ?? start.AddHours(1); // default 1 hour
            if (actualEnd <= start) throw new ArgumentException("End must be after Start.");
            return new(
                Id: Guid.NewGuid(),
                Name: name.Trim(),
                PerformanceDate: date,
                Start: start,
                End: actualEnd);
        }

        public Interval ToInterval() => new(PerformanceDate, Start, End);
    }

    /// <summary>
    /// Time interval (same day) for conflict math.
    /// </summary>
    public readonly record struct Interval(DateOnly Date, TimeOnly Start, TimeOnly End)
    {
        public bool Overlaps(Interval other)
            => Date == other.Date && Start < other.End && other.Start < End;

        public Interval? Intersection(Interval other)
            => Overlaps(other)
               ? new Interval(Date, Max(Start, other.Start), Min(End, other.End))
               : null;

        private static TimeOnly Max(TimeOnly a, TimeOnly b) => a > b ? a : b;
        private static TimeOnly Min(TimeOnly a, TimeOnly b) => a < b ? a : b;
    }

    /// <summary>
    /// A computed conflict between two artists (usually on the same stage; you can detect cross-stage conflicts too if desired).
    /// </summary>
    public record class ArtistConflict(
        Guid StageAId,
        Guid StageBId,
        Guid ArtistAId,
        Guid ArtistBId,
        DateOnly Date,
        TimeOnly OverlapStart,
        TimeOnly OverlapEnd)
    {
        public TimeSpan OverlapDuration => OverlapEnd.ToTimeSpan() - OverlapStart.ToTimeSpan();
    }

    /// <summary>
    /// Conflict detection utilities.
    /// </summary>
    public static class ConflictDetection
    {
        /// <summary>
        /// Finds pairwise conflicts within a single stage.
        /// </summary>
        public static IEnumerable<ArtistConflict> FindConflicts(Stage stage)
        {
            // Group by date so we never compare across days.
            foreach (var group in stage.Artists.GroupBy(a => a.PerformanceDate))
            {
                var byDate = group.OrderBy(a => a.Start).ToList();
                for (int i = 0; i < byDate.Count; i++)
                {
                    for (int j = i + 1; j < byDate.Count; j++)
                    {
                        var a = byDate[i];
                        var b = byDate[j];

                        var ia = a.ToInterval();
                        var ib = b.ToInterval();

                        if (!ia.Overlaps(ib))
                            break; // because list is ordered by Start, no later j will overlap with a

                        var inter = ia.Intersection(ib);
                        if (inter is not null)
                        {
                            yield return new ArtistConflict(
                                StageAId: stage.Id,
                                StageBId: stage.Id,     // same stage
                                ArtistAId: a.Id,
                                ArtistBId: b.Id,
                                Date: inter.Value.Date,
                                OverlapStart: inter.Value.Start,
                                OverlapEnd: inter.Value.End
                            );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Optional: cross-stage conflicts (useful if the user wants to watch both).
        /// </summary>
        public static IEnumerable<ArtistConflict> FindCrossStageConflicts(Stage stageA, Stage stageB)
        {
            foreach (var date in stageA.Artists.Select(a => a.PerformanceDate)
                                               .Intersect(stageB.Artists.Select(b => b.PerformanceDate)))
            {
                var aList = stageA.Artists.Where(a => a.PerformanceDate == date).OrderBy(a => a.Start).ToList();
                var bList = stageB.Artists.Where(b => b.PerformanceDate == date).OrderBy(b => b.Start).ToList();

                int i = 0, j = 0;
                while (i < aList.Count && j < bList.Count)
                {
                    var ai = aList[i].ToInterval();
                    var bj = bList[j].ToInterval();

                    if (ai.Overlaps(bj))
                    {
                        var inter = ai.Intersection(bj)!;
                        if (inter is not null)
                        {
                            var iv = inter.Value;
                            yield return new ArtistConflict(
                                StageAId: stageA.Id,
                                StageBId: stageB.Id,
                                ArtistAId: aList[i].Id,
                                ArtistBId: bList[j].Id,
                                Date: iv.Date,
                                OverlapStart: iv.Start,
                                OverlapEnd: iv.End
                            );
                            }
                    }

                    // Sweep-line style advance
                    if (ai.End <= bj.End) i++; else j++;
                }
            }
        }
    }
}
