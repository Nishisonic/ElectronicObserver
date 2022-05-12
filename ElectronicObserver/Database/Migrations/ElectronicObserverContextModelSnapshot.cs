﻿// <auto-generated />
using ElectronicObserver.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ElectronicObserver.Database.Migrations
{
    [DbContext(typeof(ElectronicObserverContext))]
    partial class ElectronicObserverContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.4");

            modelBuilder.Entity("ElectronicObserver.Window.Tools.EventLockPlanner.EventLockPlannerModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Locks")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Phases")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ShipLocks")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("EventLockPlans");
                });
#pragma warning restore 612, 618
        }
    }
}
