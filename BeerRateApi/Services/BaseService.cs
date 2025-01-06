using AutoMapper;

namespace BeerRateApi.Services
{
    /// <summary>
    /// Abstract base class for services, providing common dependencies.
    /// </summary>
    public abstract class BaseService
    {
        /// <summary>
        /// Database context for interacting with the database.
        /// </summary>
        protected readonly AppDbContext DbContext = null!;
        /// <summary>
        /// Logger for logging messages and errors.
        /// </summary>
        protected readonly ILogger Logger = null!;
        /// <summary>
        /// Mapper for object-to-object mapping.
        /// </summary>
        protected readonly IMapper Mapper = null!;

        /// <summary>
        /// Constructor for BaseService.
        /// </summary>
        /// <param name="dbContext">Database context instance.</param>
        /// <param name="logger">Logger instance for logging purposes.</param>
        /// <param name="mapper">Mapper instance for mapping objects.</param>
        public BaseService(AppDbContext dbContext, ILogger logger, IMapper mapper)
        {
            DbContext = dbContext;
            Logger = logger;
            Mapper = mapper;
        }
    }
}
