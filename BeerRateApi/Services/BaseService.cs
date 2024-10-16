using AutoMapper;

namespace BeerRateApi.Services
{
    public abstract class BaseService
    {
        protected readonly AppDbContext DbContext = null!;
        protected readonly ILogger Logger = null!;
        public BaseService(AppDbContext dbContext, ILogger logger)
        {
            DbContext = dbContext;
            Logger = logger;
        }
    }
}
