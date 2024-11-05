using AutoMapper;
using Flocus.Identity.Models;
using Flocus.Models.Requests;

namespace Flocus.Mapping;

public sealed class RegistrationModelMappingProfile : Profile
{
    public RegistrationModelMappingProfile() 
    {
        CreateMap<RegisterRequestDto, RegistrationModel>();
    }
}
