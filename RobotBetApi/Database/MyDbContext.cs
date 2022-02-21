using Microsoft.EntityFrameworkCore;
using RobotBetApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotBetApi.Database
{
    public partial class MyDbContext : DbContext
    {
        public DbSet<Pilot> Pilots { get; set; }
        public DbSet<Race> Races { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pilot>()
                .HasIndex(p => new { p.RaceId, p.PilotCode })
                .IsUnique(true);
            modelBuilder.Entity<Pilot>()
                .HasOne(p => p.Race)
                .WithMany(r => r.Pilots)
                .HasForeignKey(p => p.RaceId);
        }
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }
    }
}
