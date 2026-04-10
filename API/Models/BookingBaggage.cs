using System;

namespace API.Models;

public class BookingBaggage
{
    public int Id { get; set; }

    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public int BaggageTypeId { get; set; }
    public BaggageType BaggageType { get; set; } = null!;
}
