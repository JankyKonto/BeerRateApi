using BeerRateApi.DTOs;
using BeerRateApi.Models;
using System.Linq.Expressions;

namespace BeerRateApi.Interfaces
{
    public interface IUserService
    {
        Task<RegisterResult> RegisterUser(RegisterDTO registerDTO);
        Task<LoginResult> LoginUser (LoginDTO loginDTO);
        ResetResult ResetPassword (ResetDTO resetDTO);
        Task<LoginResult> Refresh(string expiredToken, string refreshToken);
        Task Revoke(int id);
    }
}
