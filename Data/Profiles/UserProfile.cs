using activitiesapp.Models;
using activitiesapp.Models.UserDTO;
using AutoMapper;

namespace activitiesapp.Data.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserFullDTO>()
                .ReverseMap();
            CreateMap<User, UserLoginDTO>()
                .ReverseMap();
            CreateMap<User, UserUpdateDTO>()
                .ReverseMap();
        }
    }
}