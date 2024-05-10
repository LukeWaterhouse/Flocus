using AutoMapper;
using Flocus.Repository.Interfaces;
using Flocus.Repository.Mapping;
using Flocus.Repository.Services;
using NSubstitute;

namespace Flocus.Repository.Tests.Services.Repository;

public class GetUserTests
{
    private readonly IMapper _mapper;
    private readonly IDbConnectionService _dbConnectionService;
    private readonly ISqlQueryService _sqlQueryService;
    private readonly RepositoryService _repositoryService;

    public GetUserTests()
    {

        var mappingConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(UserMappingProfile));
        });

        _mapper = mappingConfig.CreateMapper();
        _dbConnectionService = Substitute.For<IDbConnectionService>();
        _sqlQueryService = Substitute.For<ISqlQueryService>();
        _repositoryService = new RepositoryService(_mapper, _dbConnectionService, _sqlQueryService);
    }

    //[Fact]
    //public async

}
