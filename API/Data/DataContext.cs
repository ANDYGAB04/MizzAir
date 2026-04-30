using System;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<User, AppRole, int,
     IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>,
     IdentityUserToken<int>>(options)
{
    public DbSet<Aircraft> Aircrafts { get; set; }
    public DbSet<Airport> Airports { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BaggageType> BaggageTypes { get; set; }
    public DbSet<BookingBaggage> BookingBaggages { get; set; }
    public DbSet<BookingSeat> BookingSeats { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Seat> Seats { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        builder.Entity<AppRole>()
          .HasMany(ur => ur.UserRoles)
          .WithOne(u => u.Role)
          .HasForeignKey(ur => ur.RoleId)
          .IsRequired();

        builder.Entity<Booking>()
            .HasIndex(b => b.BookingReference)
            .IsUnique();

        builder.Entity<Aircraft>()
            .HasIndex(a => a.RegistrationNumber)
            .IsUnique();

        builder.Entity<Flight>().Property(f => f.Price).HasPrecision(10, 2);
        builder.Entity<Booking>().Property(b => b.TotalPrice).HasPrecision(10, 2);
        builder.Entity<BaggageType>().Property(b => b.Price).HasPrecision(10, 2);
        builder.Entity<BaggageType>().Property(b => b.MaxWeight).HasPrecision(5, 2);

        // Configure Flight-Airport relationships
        builder.Entity<Flight>()
            .HasOne(f => f.DepartureAirport)
            .WithMany(a => a.DepartureFlights)
            .HasForeignKey(f => f.DepartureAirportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Flight>()
            .HasOne(f => f.ArrivalAirport)
            .WithMany(a => a.ArrivalFlights)
            .HasForeignKey(f => f.ArrivalAirportId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
