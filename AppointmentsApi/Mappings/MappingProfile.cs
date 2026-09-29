using AutoMapper;
using InnoAppointmentsApi.Dtos;
using InnoAppointmentsApi.Entities;
using InnoAppointmentsApi.Features.Appointments;
using InnoAppointmentsApi.Features.Results;
using InnoAppointmentsApi.Features.Schedules;

namespace InnoAppointmentsApi.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Appointment, AppointmentDto>();
        CreateMap<CreateAppointmentCommand, Appointment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsApproved, opt => opt.Ignore());
        CreateMap<UpdateAppointmentCommand, Appointment>();
        
        CreateMap<Result, ResultDto>();
        CreateMap<CreateResultCommand, Result>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateResultCommand, Result>();
        
        CreateMap<WorkDay, WorkDayDto>().ReverseMap();
        CreateMap<Schedule, ScheduleDto>();
        
        CreateMap<CreateScheduleCommand, Schedule>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateScheduleCommand, Schedule>();
    }
}