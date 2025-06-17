using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalMapper.App.Models
{
    public class FestivalMap
    {
        public string Id { get; set; }
        public string FestivalName { get; set; }
        public string FestivalMapBase64 { get; set; }
        public DateTime FestivalStartDate { get; set; } = DateTime.Now;
        public List<Stage> Stages { get; set; } = new List<Stage>();

    }

    public class Stage
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string StageName { get; set; }
        public Point StageLocation { get; set; }
        public string StageColor { get; set; }
        public string StageBg { get
            {
                return $"{StageColor}bf";
            } 
        }
        public List<Artist> Artists { get; set; } = new List<Artist>();
    }

    public class Artist
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public DateTime SetTimeStart { get; set; }
        public DateTime SetTimeEnd { get; set; }
        public short? SetDurationMinutes { get; set; }
        public List<string> Conflicts { get; set; } = new List<string>();
    }

}
