using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotBetApi.Models
{
    public class PilotDto
    {
        //public int RaceId { get; set; }
        public int PilotCode { get; set; } // 1 - Verde | 2 - Vermelho | 3 - Amarelo | 4 - Roxo
        public string PilotName { get; set; }
        public double Odd { get; set; }

        //public RaceDto RaceDto { get; set; }
    }
}
