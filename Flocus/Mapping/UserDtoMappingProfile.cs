using AutoMapper;
using Flocus.Domain.Models;
using Flocus.Models.ReturnModels;

namespace Flocus.Mapping;

public class UserDtoMappingProfile : Profile
{
    public UserDtoMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(
                result => result.Username,
                opt => opt.MapFrom(src => src.Username))
            .ForMember(
                result => result.EmailAddress,
                opt => opt.MapFrom(src => src.EmailAddress))
            .ForMember(
                result => result.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt));
    }
}
