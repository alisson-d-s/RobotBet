using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RobotBetApi.Database;
using RobotBetApi.Models;

namespace RobotBetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RacesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public RacesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Races
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RaceDto>>> GetRaces()
        {
            var races = from r in _context.Races
                        select new RaceDto()
                        {
                            RaceId = r.RaceId,
                            RaceDate = r.RaceDate,
                            Pilots = from p in r.Pilots
                                     select new PilotDto()
                                     {
                                         PilotCode = p.PilotCode,
                                         PilotName = p.PilotName,
                                         Odd = p.Odd
                                     }
                        };
            return await races.ToListAsync();
        }

        // GET: api/Races/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RaceDto>> GetRace(int id)
        {
            var raceDto = await _context.Races.Include(r => r.Pilots).Select(r =>
                new RaceDto()
                {
                    RaceId = r.RaceId,
                    RaceDate = r.RaceDate,
                    Pilots = from p in r.Pilots
                             select new PilotDto()
                             {
                                 PilotCode = p.PilotCode,
                                 PilotName = p.PilotName,
                                 Odd = p.Odd
                             }
                }).SingleOrDefaultAsync(r => r.RaceId == id);
            //var race = await _context.Races.FindAsync(id);

            if (raceDto == null)
            {
                return NotFound();
            }

            return Ok(raceDto);
        }

        // PUT: api/Races/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRace(int id, Race race)
        {
            if (id != race.RaceId)
            {
                return BadRequest();
            }

            _context.Entry(race).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RaceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Races
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RaceDto>> PostRace(Race race)
        {
            _context.Races.Add(race);
            await _context.SaveChangesAsync();

            var raceDto = new RaceDto()
            {
                RaceId = race.RaceId,
                RaceDate = race.RaceDate,
                Pilots = from p in race.Pilots
                         select new PilotDto()
                         {
                             PilotCode = p.PilotCode,
                             PilotName = p.PilotName,
                             Odd = p.Odd
                         }
            };
            return CreatedAtAction("GetRace", new { id = race.RaceId }, raceDto);
        }

        // DELETE: api/Races/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRace(int id)
        {
            var race = await _context.Races.FindAsync(id);
            if (race == null)
            {
                return NotFound();
            }

            _context.Races.Remove(race);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RaceExists(int id)
        {
            return _context.Races.Any(e => e.RaceId == id);
        }
    }
}
