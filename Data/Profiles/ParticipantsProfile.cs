using activitiesapp.Models;
using AutoMapper;

namespace activitiesapp.Data.Profiles
{
    public class ParticipantsProfile : Profile
    {
        public ParticipantsProfile()
        {
            CreateMap<Participants, ParticipantsDTO>()
                .ReverseMap();
        }
    }
}