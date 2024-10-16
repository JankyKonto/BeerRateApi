using AutoMapper;
using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Models;
using Microsoft.AspNetCore.Identity;

namespace BeerRateApi.Services
{
    public class UserService : BaseService, IUserService
    {
        public UserService(AppDbContext dbContext, IMapper mapper, ILogger logger, UserManager<User> userManager) : base(dbContext, mapper, logger)
        {
        }

        public RegisterResult RegisterUser (RegisterDTO registerDTO)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
