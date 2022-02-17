using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RobotBetApi.Models
{
    public class Race
    {
        [Key]
        [Required]
        public int RaceId { get; set; }
        [Required()]
        public DateTimeOffset RaceDate { get; set; }

        public virtual ICollection<Pilot> Pilots { get; set; }
    }
}
