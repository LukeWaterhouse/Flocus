using AutoMapper;
using Flocus.Domain.Models.Errors;
using Flocus.Models.Errors;

namespace Flocus.Mapping;

public class ErrorDtoMappingProfile : Profile
{
    public ErrorDtoMappingProfile()
    {
        CreateMap<Error, ErrorDto>();
    }
}
