using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotBetApi.Models
{
    public class RaceDto
    {
        public int RaceId { get; set; }
        public DateTimeOffset RaceDate { get; set; }

        public IEnumerable<PilotDto> Pilots { get; set; }
    }
}
