using activitiesapp.Models;
using activitiesapp.Models.EventDTO;
using AutoMapper;

namespace activitiesapp.Data.Profiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, EventFullDTO>()
                .ReverseMap();
            CreateMap<Event, EventUpdateDTO>()
                .ReverseMap();
        }
    }
}