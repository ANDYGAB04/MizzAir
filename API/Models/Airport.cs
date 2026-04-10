using System;

namespace API.Models;

public class Airport
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public required string IATACode { get; set; }
    public List<Flight> DepartureFlights { get; set; } = [];
    public List<Flight> ArrivalFlights { get; set; } = [];
}
