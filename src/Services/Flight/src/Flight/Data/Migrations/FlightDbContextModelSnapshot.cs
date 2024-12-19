﻿// <auto-generated />
using System;
using Flight.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Flight.Data.Migrations
{
    [DbContext(typeof(FlightDbContext))]
    partial class FlightDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Flight.Aircrafts.Models.Aircraft", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<long?>("CreatedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("created_by");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified");

                    b.Property<long?>("LastModifiedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("last_modified_by");

                    b.Property<long>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("bigint")
                        .HasColumnName("version");

                    b.HasKey("Id")
                        .HasName("pk_aircraft");

                    b.ToTable("aircraft", (string)null);
                });

            modelBuilder.Entity("Flight.Airports.Models.Airport", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<long?>("CreatedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("created_by");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified");

                    b.Property<long?>("LastModifiedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("last_modified_by");

                    b.Property<long>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("bigint")
                        .HasColumnName("version");

                    b.HasKey("Id")
                        .HasName("pk_airport");

                    b.ToTable("airport", (string)null);
                });

            modelBuilder.Entity("Flight.Flights.Models.Flight", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("AircraftId")
                        .HasColumnType("uuid")
                        .HasColumnName("aircraft_id");

                    b.Property<Guid>("ArriveAirportId")
                        .HasColumnType("uuid")
                        .HasColumnName("arrive_airport_id");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<long?>("CreatedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("created_by");

                    b.Property<Guid>("DepartureAirportId")
                        .HasColumnType("uuid")
                        .HasColumnName("departure_airport_id");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified");

                    b.Property<long?>("LastModifiedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("last_modified_by");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("Unknown")
                        .HasColumnName("status");

                    b.Property<long>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("bigint")
                        .HasColumnName("version");

                    b.HasKey("Id")
                        .HasName("pk_flight");

                    b.HasIndex("AircraftId")
                        .HasDatabaseName("ix_flight_aircraft_id");

                    b.HasIndex("ArriveAirportId")
                        .HasDatabaseName("ix_flight_arrive_airport_id");

                    b.HasIndex("DepartureAirportId")
                        .HasDatabaseName("ix_flight_departure_airport_id");

                    b.ToTable("flight", (string)null);
                });

            modelBuilder.Entity("Flight.Seats.Models.Seat", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Class")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("Unknown")
                        .HasColumnName("class");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<long?>("CreatedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("created_by");

                    b.Property<Guid>("FlightId")
                        .HasColumnType("uuid")
                        .HasColumnName("flight_id");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified");

                    b.Property<long?>("LastModifiedBy")
                        .HasColumnType("bigint")
                        .HasColumnName("last_modified_by");

                    b.Property<string>("Type")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("Unknown")
                        .HasColumnName("type");

                    b.Property<long>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("bigint")
                        .HasColumnName("version");

                    b.HasKey("Id")
                        .HasName("pk_seat");

                    b.HasIndex("FlightId")
                        .HasDatabaseName("ix_seat_flight_id");

                    b.ToTable("seat", (string)null);
                });

            modelBuilder.Entity("Flight.Aircrafts.Models.Aircraft", b =>
                {
                    b.OwnsOne("Flight.Aircrafts.ValueObjects.ManufacturingYear", "ManufacturingYear", b1 =>
                        {
                            b1.Property<Guid>("AircraftId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<int>("Value")
                                .HasMaxLength(5)
                                .HasColumnType("integer")
                                .HasColumnName("manufacturing_year");

                            b1.HasKey("AircraftId")
                                .HasName("pk_aircraft");

                            b1.ToTable("aircraft");

                            b1.WithOwner()
                                .HasForeignKey("AircraftId")
                                .HasConstraintName("fk_aircraft_aircraft_id");
                        });

                    b.OwnsOne("Flight.Aircrafts.ValueObjects.Model", "Model", b1 =>
                        {
                            b1.Property<Guid>("AircraftId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("model");

                            b1.HasKey("AircraftId")
                                .HasName("pk_aircraft");

                            b1.ToTable("aircraft");

                            b1.WithOwner()
                                .HasForeignKey("AircraftId")
                                .HasConstraintName("fk_aircraft_aircraft_id");
                        });

                    b.OwnsOne("Flight.Aircrafts.ValueObjects.Name", "Name", b1 =>
                        {
                            b1.Property<Guid>("AircraftId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("name");

                            b1.HasKey("AircraftId")
                                .HasName("pk_aircraft");

                            b1.ToTable("aircraft");

                            b1.WithOwner()
                                .HasForeignKey("AircraftId")
                                .HasConstraintName("fk_aircraft_aircraft_id");
                        });

                    b.Navigation("ManufacturingYear")
                        .IsRequired();

                    b.Navigation("Model")
                        .IsRequired();

                    b.Navigation("Name")
                        .IsRequired();
                });

            modelBuilder.Entity("Flight.Airports.Models.Airport", b =>
                {
                    b.OwnsOne("Flight.Airports.ValueObjects.Address", "Address", b1 =>
                        {
                            b1.Property<Guid>("AirportId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("address");

                            b1.HasKey("AirportId")
                                .HasName("pk_airport");

                            b1.ToTable("airport");

                            b1.WithOwner()
                                .HasForeignKey("AirportId")
                                .HasConstraintName("fk_airport_airport_id");
                        });

                    b.OwnsOne("Flight.Airports.ValueObjects.Code", "Code", b1 =>
                        {
                            b1.Property<Guid>("AirportId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("code");

                            b1.HasKey("AirportId")
                                .HasName("pk_airport");

                            b1.ToTable("airport");

                            b1.WithOwner()
                                .HasForeignKey("AirportId")
                                .HasConstraintName("fk_airport_airport_id");
                        });

                    b.OwnsOne("Flight.Airports.ValueObjects.Name", "Name", b1 =>
                        {
                            b1.Property<Guid>("AirportId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("name");

                            b1.HasKey("AirportId")
                                .HasName("pk_airport");

                            b1.ToTable("airport");

                            b1.WithOwner()
                                .HasForeignKey("AirportId")
                                .HasConstraintName("fk_airport_airport_id");
                        });

                    b.Navigation("Address")
                        .IsRequired();

                    b.Navigation("Code")
                        .IsRequired();

                    b.Navigation("Name")
                        .IsRequired();
                });

            modelBuilder.Entity("Flight.Flights.Models.Flight", b =>
                {
                    b.HasOne("Flight.Aircrafts.Models.Aircraft", null)
                        .WithMany()
                        .HasForeignKey("AircraftId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_flight_aircraft_aircraft_id");

                    b.HasOne("Flight.Airports.Models.Airport", null)
                        .WithMany()
                        .HasForeignKey("ArriveAirportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_flight_airport_arrive_airport_id");

                    b.HasOne("Flight.Airports.Models.Airport", null)
                        .WithMany()
                        .HasForeignKey("DepartureAirportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_flight_airport_departure_airport_id");

                    b.OwnsOne("Flight.Flights.ValueObjects.ArriveDate", "ArriveDate", b1 =>
                        {
                            b1.Property<Guid>("FlightId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<DateTime>("Value")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("arrive_date");

                            b1.HasKey("FlightId")
                                .HasName("pk_flight");

                            b1.ToTable("flight");

                            b1.WithOwner()
                                .HasForeignKey("FlightId")
                                .HasConstraintName("fk_flight_flight_id");
                        });

                    b.OwnsOne("Flight.Flights.ValueObjects.DepartureDate", "DepartureDate", b1 =>
                        {
                            b1.Property<Guid>("FlightId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<DateTime>("Value")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("departure_date");

                            b1.HasKey("FlightId")
                                .HasName("pk_flight");

                            b1.ToTable("flight");

                            b1.WithOwner()
                                .HasForeignKey("FlightId")
                                .HasConstraintName("fk_flight_flight_id");
                        });

                    b.OwnsOne("Flight.Flights.ValueObjects.DurationMinutes", "DurationMinutes", b1 =>
                        {
                            b1.Property<Guid>("FlightId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<decimal>("Value")
                                .HasMaxLength(50)
                                .HasColumnType("numeric")
                                .HasColumnName("duration_minutes");

                            b1.HasKey("FlightId")
                                .HasName("pk_flight");

                            b1.ToTable("flight");

                            b1.WithOwner()
                                .HasForeignKey("FlightId")
                                .HasConstraintName("fk_flight_flight_id");
                        });

                    b.OwnsOne("Flight.Flights.ValueObjects.FlightDate", "FlightDate", b1 =>
                        {
                            b1.Property<Guid>("FlightId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<DateTime>("Value")
                                .HasColumnType("timestamp with time zone")
                                .HasColumnName("flight_date");

                            b1.HasKey("FlightId")
                                .HasName("pk_flight");

                            b1.ToTable("flight");

                            b1.WithOwner()
                                .HasForeignKey("FlightId")
                                .HasConstraintName("fk_flight_flight_id");
                        });

                    b.OwnsOne("Flight.Flights.ValueObjects.FlightNumber", "FlightNumber", b1 =>
                        {
                            b1.Property<Guid>("FlightId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("flight_number");

                            b1.HasKey("FlightId")
                                .HasName("pk_flight");

                            b1.ToTable("flight");

                            b1.WithOwner()
                                .HasForeignKey("FlightId")
                                .HasConstraintName("fk_flight_flight_id");
                        });

                    b.OwnsOne("Flight.Flights.ValueObjects.Price", "Price", b1 =>
                        {
                            b1.Property<Guid>("FlightId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<decimal>("Value")
                                .HasMaxLength(10)
                                .HasColumnType("numeric")
                                .HasColumnName("price");

                            b1.HasKey("FlightId")
                                .HasName("pk_flight");

                            b1.ToTable("flight");

                            b1.WithOwner()
                                .HasForeignKey("FlightId")
                                .HasConstraintName("fk_flight_flight_id");
                        });

                    b.Navigation("ArriveDate")
                        .IsRequired();

                    b.Navigation("DepartureDate")
                        .IsRequired();

                    b.Navigation("DurationMinutes")
                        .IsRequired();

                    b.Navigation("FlightDate")
                        .IsRequired();

                    b.Navigation("FlightNumber")
                        .IsRequired();

                    b.Navigation("Price")
                        .IsRequired();
                });

            modelBuilder.Entity("Flight.Seats.Models.Seat", b =>
                {
                    b.HasOne("Flight.Flights.Models.Flight", null)
                        .WithMany()
                        .HasForeignKey("FlightId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_seat_flight_flight_id");

                    b.OwnsOne("Flight.Seats.ValueObjects.SeatNumber", "SeatNumber", b1 =>
                        {
                            b1.Property<Guid>("SeatId")
                                .HasColumnType("uuid")
                                .HasColumnName("id");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("seat_number");

                            b1.HasKey("SeatId")
                                .HasName("pk_seat");

                            b1.ToTable("seat");

                            b1.WithOwner()
                                .HasForeignKey("SeatId")
                                .HasConstraintName("fk_seat_seat_id");
                        });

                    b.Navigation("SeatNumber")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
