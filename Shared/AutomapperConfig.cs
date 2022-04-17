using AutoMapper;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Shared;

public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        CreateMap<EventRegistration, EventRegistrationDto>();
        CreateMap<Announcement, AnnouncementDto>();
        CreateMap<TrainingTicket, TrainingTicketDto>();
        CreateMap<User, UserDto>();
        CreateMap<ControllerLog, ControllerLogDto>();
        CreateMap<Hours, HoursDto>();
        CreateMap<SoloCert, SoloCertDto>();
    }
}