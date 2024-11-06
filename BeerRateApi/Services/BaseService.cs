using AutoMapper;

namespace BeerRateApi.Services
{
    public abstract class BaseService
    {
        protected readonly AppDbContext DbContext = null!;
        protected readonly ILogger Logger = null!;
        protected readonly IMapper Mapper = null!;

        public BaseService(AppDbContext dbContext, ILogger logger, IMapper mapper)
        {
            DbContext = dbContext;
            Logger = logger;
            Mapper = mapper;
        }
    }
}
