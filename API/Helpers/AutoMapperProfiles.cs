using System;
using API.DTOs;
using API.Models;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<RegisterDto, User>();
        CreateMap<Airport, AirportDto>();
        CreateMap<Flight, FlightDto>()
            .ForMember(dest => dest.AircraftType, o => o.MapFrom(src => src.Aircraft.Model))
            .ForMember(dest => dest.DepartureAirportName, o => o.MapFrom(src => src.DepartureAirport.Name))
            .ForMember(dest => dest.ArrivalAirportName, o => o.MapFrom(src => src.ArrivalAirport.Name));
        CreateMap<BaggageType, BaggageTypeDto>();
        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.SeatNumbers, o => o.MapFrom(src => src.BookingSeats
                .Select(bs => bs.Seat.SeatNumber)
                .ToList()
                ))
            .ForMember(dest => dest.DepartureTime, o => o.MapFrom(src => src.Flight.DepartureTime))
            .ForMember(dest => dest.ArrivalTime, o => o.MapFrom(src => src.Flight.ArrivalTime));
    }
}
