using System;

namespace API.Interface;

public interface INotificationService
{
    Task<int> BroadcastAnnouncement(int adminUserId, string message, string type);
    Task<int> NotifyFlightStatusChange(int flightId, string flightNumber, string newStatus);
}

