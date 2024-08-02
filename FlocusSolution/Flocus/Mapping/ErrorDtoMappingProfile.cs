using AutoMapper;
using Flocus.Domain.Models.Errors;
using Flocus.Models.Errors;

namespace Flocus.Mapping;

public sealed class ErrorDtoMappingProfile : Profile
{
    public ErrorDtoMappingProfile()
    {
        CreateMap<Error, ErrorDto>();
    }
}
