using AutoMapper;
using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

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
                return new RegisterResult();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public LoginResult LoginUser(LoginDTO loginDTO)
        {
            try
            {
                return new LoginResult();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public IEnumerable<UserDTO> GetUsers(Expression<Func<User, bool>> filterExpression = null)
        {
            try
            {
                var userEntities = DbContext.Users.AsQueryable();
                if (filterExpression != null)
                    userEntities = userEntities.Where(filterExpression);
                var userDTOs = Mapper.Map<IEnumerable<UserDTO>>(userEntities);
                return userDTOs;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public UserDTO GetUser(Expression<Func<User, bool>> filterExpression)
        {
            try
            {
                if (filterExpression == null)
                    throw new ArgumentNullException($" FilterExpression is null");
                var userEntity = DbContext.Users.FirstOrDefault(filterExpression);
                var userDTO = Mapper.Map<User, UserDTO>(userEntity);
                return userDTO;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }

        }

        public ResetResult ResetPassword(ResetDTO resetDTO)
        {
            throw new NotImplementedException();
        }
    }
}
