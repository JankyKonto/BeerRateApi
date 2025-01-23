using BeerRateApi.DTOs;
using BeerRateApi.Models;
using System.Linq.Expressions;

namespace BeerRateApi.Interfaces
{
    /// <summary>
    /// An interface for service for managing Users.
    /// Provides methods for user registration, login, password management, token handling, and email operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDTO">The user registration details.</param>
        /// <returns>A <see cref="RegisterResult"/> containing the registered username.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the username or email already exists.</exception>
        /// <exception cref="ArgumentException">Thrown if the password is too short or the username contains forbidden characters.</exception>
        Task<RegisterResult> RegisterUser(RegisterDTO registerDTO);

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="loginDTO">The user login credentials.</param>
        /// <returns>A <see cref="LoginResult"/> containing user details and tokens.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if the email or password is invalid.</exception>
        Task<LoginResult> LoginUser (LoginDTO loginDTO);

        /// <summary>
        /// Refreshes a user's JWT token.
        /// </summary>
        /// <param name="expiredToken">The expired JWT token.</param>
        /// <param name="refreshToken">The associated refresh token.</param>
        /// <returns>A <see cref="LoginResult"/> with a refreshed JWT token.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown for invalid or expired tokens.</exception>
        Task<LoginResult> Refresh(string expiredToken, string refreshToken);

        /// <summary>
        /// Revokes a user's refresh token.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the user is not found.</exception>
        Task Revoke(int id);

        /// <summary>
        /// Resets a user's password using a token.
        /// </summary>
        /// <param name="newPassword">The new password.</param>
        /// <param name="token">The reset token.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown for invalid or expired tokens.</exception>
        /// <exception cref="ArgumentException">Thrown if the password is too short.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the password or token is null.</exception>
        Task RealisePasswordReminding(string newPassword, string token);

        /// <summary>
        /// Sends a password reminder email to the user.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown if the user is not found.</exception>
        Task RemindPasswordSendEmail(string email);
    }
}
