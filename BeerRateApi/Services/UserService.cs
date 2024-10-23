using BeerRateApi.DTOs;
using BeerRateApi.Interfaces;
using BeerRateApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace BeerRateApi.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly ITokenService _tokenService;

        public UserService(AppDbContext dbContext,  ILogger logger, ITokenService tokenService) : base(dbContext, logger)
        {
            _tokenService = tokenService;
        }

        public async Task<RegisterResult> RegisterUser (RegisterDTO registerDTO)
        {
            try
            {
                if (await DbContext.Users.AnyAsync(user => user.Username == registerDTO.Username))
                {
                    throw new Exception();
                }

                if (await DbContext.Users.AnyAsync(user => user.Email == registerDTO.Email))
                {
                    throw new Exception();
                }

                var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(registerDTO.Password);
                var user = new User { Email = registerDTO.Email, Username = registerDTO.Username, PasswordHash=passwordHash };
                DbContext.Users.Add(user);
                await DbContext.SaveChangesAsync();
                return new RegisterResult{Username = registerDTO.Username};
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<LoginResult> LoginUser(LoginDTO loginDTO)
        {
            try
            {
                var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email==loginDTO.Email);

                if (user==null)
                    throw new Exception("User is null");

                if (!BCrypt.Net.BCrypt.EnhancedVerify(loginDTO.Password, user.PasswordHash))
                {
                    throw new Exception("Verification unsuccesful");
                }

                var refreshToken = _tokenService.GenerateRefreshToken();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = refreshTokenExpiry;
                await DbContext.SaveChangesAsync();

                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = refreshTokenExpiry
                };

                var jwtToken = _tokenService.GenerateJwtToken(user.Username, user.Id);

                return new LoginResult { Id=user.Id, Email=user.Email, Username=user.Username, JwtToken=jwtToken, RefreshTokenExpiry=user.RefreshTokenExpiry, RefreshToken=user.RefreshToken };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /* public IEnumerable<UserDTO> GetUsers(Expression<Func<User, bool>> filterExpression = null)
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

        } */

        public ResetResult ResetPassword(ResetDTO resetDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResult> Refresh(string expiredToken, string refreshToken) 
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(expiredToken);
                if (principal?.Identity?.Name == null)
                {
                    throw new Exception();
                }

                var user = await DbContext.Users.FirstOrDefaultAsync(user => user.Username == principal.Identity.Name);
                if (user == null || refreshToken == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    throw new Exception();
                }

                var email = user.Email;
                if (email == null)
                {
                    throw new Exception();
                }

                var idClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (idClaim == null) 
                {
                    throw new Exception();
                }

                if (!int.TryParse(idClaim.Value, out var id)) 
                { 
                    throw new Exception(); 
                }

                var jwtToken = _tokenService.GenerateJwtToken(principal.Identity.Name,id);

                return new LoginResult { Id = id, Username = principal.Identity.Name, JwtToken=jwtToken, Email=email };
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }



    }
}
