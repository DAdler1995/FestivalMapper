using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalMapper.App.Models
{
    public class FestivalMap
    {
        public string FestivalName { get; set; }
        public string Description { get; set; }
        public string FestivalMapBase64 { get; set; }
        public List<Stage> Stages { get; set; } = new List<Stage>();

    }

    public class Stage
    {
        public int Id { get; set; }
        public string StageName { get; set; }
        public Point StageLocation { get; set; }
        public string StageDescription { get; set; }
        public List<Artist> Artists { get; set; } = new List<Artist>();
    }

    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime SetTimeStart { get; set; }
        public DateTime? SetTimeEnd { get; set; }
        public short? SetDurationMinutes { get; set; }
    }
}
