using AutoMapper;
using Flocus.Domain.Models;
using Flocus.Repository.Models;

namespace Flocus.Repository.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<DbUser, User>()
            .ForMember(
                result => result.ClientId,
                opt => opt.MapFrom(src => src.Client_id))
            .ForMember(
                result => result.CreatedAt,
                opt => opt.MapFrom(src => src.Account_creation_date))
            .ForMember(
                result => result.Username,
                opt => opt.MapFrom(src => src.Username))
            .ForMember(
                result => result.IsAdmin,
                opt => opt.MapFrom(src => src.Admin_rights))
            .ForMember(
                result => result.PasswordHash,
                opt => opt.MapFrom(src => src.Password_hash));
    }
}
