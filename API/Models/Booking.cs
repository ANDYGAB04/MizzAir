using System;

namespace API.Models;

public class Booking
{
    public int Id { get; set; }
    public required string BookingReference { get; set; }
    public DateTime BookingDate { get; set; }
    public required string Status { get; set; }
    public decimal TotalPrice { get; set; }
    public List<BookingSeat> BookingSeats { get; set; } = [];
    public List<BookingBaggage> BookingBaggages { get; set; } = [];

    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int FlightId { get; set; }
    public Flight Flight { get; set; } = null!;
}
