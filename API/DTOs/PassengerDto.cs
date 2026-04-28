namespace API.DTOs;

public class PassengerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public int TotalBookings { get; set; }
    public DateTime? LastBookingDate { get; set; }
}
