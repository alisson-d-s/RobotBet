using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RobotBetApi.Models
{
    public class Pilot
    {
        [Key]
        [Required()]
        public int Id { get; set; }
        public int RaceId { get; set; } 
        [Required()]
        public int PilotCode { get; set; } // 1 - Verde | 2 - Vermelho | 3 - Amarelo | 4 - Roxo
        [Required()]
        [StringLength(100, MinimumLength = 1)]
        public string PilotName { get; set; }
        [Required()]
        public double Odd { get; set; }

        public virtual Race Race { get; set; }
    }
}
