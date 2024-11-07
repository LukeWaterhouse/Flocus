using AutoMapper;
using Flocus.Domain.Models;
using Flocus.Repository.Models;

namespace Flocus.Repository.Mapping;

public sealed class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<DbUser, User>()
            .ForCtorParam(
                nameof(User.ClientId),
                opt => opt.MapFrom(src => src.Client_id))
            .ForCtorParam(
                nameof(User.EmailAddress),
                opt => opt.MapFrom(src => src.Email_address))
            .ForCtorParam(
                nameof(User.CreatedAt),
                opt => opt.MapFrom(src => src.Account_creation_date))
            .ForCtorParam(
                nameof(User.Username),
                opt => opt.MapFrom(src => src.Username))
            .ForCtorParam(
                nameof(User.IsAdmin),
                opt => opt.MapFrom(src => src.Admin_rights))
            .ForCtorParam(
                nameof(User.PasswordHash),
                opt => opt.MapFrom(src => src.Password_hash));
    }
}
