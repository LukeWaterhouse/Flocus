using AutoMapper;
using Flocus.Domain.Models;
using Flocus.Models.ReturnModels.UserReturnModels;

namespace Flocus.Mapping;

public sealed class UserDtoMappingProfile : Profile
{
    public UserDtoMappingProfile()
    {
        CreateMap<User, UserBasicInfoDto>();
        CreateMap<User, UserSensitiveInfoDto>();
    }
}
