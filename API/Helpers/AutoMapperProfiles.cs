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
        CreateMap<Flight, FlightDto>()
            .ForMember(dest => dest.AircraftType, o => o.MapFrom(src => src.Aircraft.Model))
            .ForMember(dest => dest.DepartureAirportName, o => o.MapFrom(src => src.DepartureAirport.Name))
            .ForMember(dest => dest.ArrivalAirportName, o => o.MapFrom(src => src.ArrivalAirport.Name));
    }
}
