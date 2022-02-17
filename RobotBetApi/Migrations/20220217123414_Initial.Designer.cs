﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RobotBetApi.Database;

namespace RobotBetApi.Migrations
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20220217123414_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.14");

            modelBuilder.Entity("RobotBetApi.Models.Pilot", b =>
                {
                    b.Property<int>("PilotId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<double>("Odd")
                        .HasColumnType("double");

                    b.Property<int>("PilotCode")
                        .HasColumnType("int");

                    b.Property<string>("PilotName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("RaceId")
                        .HasColumnType("int");

                    b.HasKey("PilotId");

                    b.HasIndex("RaceId");

                    b.ToTable("Pilots");
                });

            modelBuilder.Entity("RobotBetApi.Models.Race", b =>
                {
                    b.Property<int>("RaceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("RaceDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("RaceId");

                    b.ToTable("Races");
                });

            modelBuilder.Entity("RobotBetApi.Models.Pilot", b =>
                {
                    b.HasOne("RobotBetApi.Models.Race", "Race")
                        .WithMany("Pilots")
                        .HasForeignKey("RaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Race");
                });

            modelBuilder.Entity("RobotBetApi.Models.Race", b =>
                {
                    b.Navigation("Pilots");
                });
#pragma warning restore 612, 618
        }
    }
}