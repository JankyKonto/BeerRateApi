using BeerRateApi.DTOs;
using BeerRateApi.Models;
using System.Linq.Expressions;

namespace BeerRateApi.Interfaces
{
    public interface IUserService
    {
        RegisterResult RegisterUser(RegisterDTO registerDTO);
        LoginResult LoginUser (LoginDTO loginDTO);
        IEnumerable<UserDTO> GetUsers(Expression<Func<User, bool>> filterExpression = null);
        UserDTO GetUser(Expression<Func<User, bool>> filterExpression);
        ResetResult ResetPassword (ResetDTO resetDTO);
    }
}
