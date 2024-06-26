using Xunit;
using StockApp.Application.Services;
using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Moq;
using System.Security.Authentication;
using StockApp.Domain.Interfaces;

namespace StockApp.AuthServiceTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<AuthService>>();

            _configurationMock.Setup(config => config["JwtSettings:SecretKey"]).Returns("ChaveSecretaParaJwtTokenQueTemQueTerMaisQue32CaractereParaFuncionar");
            _configurationMock.Setup(config => config["JwtSettings:Issuer"]).Returns("SeuIssuer");
            _configurationMock.Setup(config => config["JwtSettings:Audience"]).Returns("SuaAudience");
            _configurationMock.Setup(config => config["JwtSettings:AccessTokenExpirationMinutes"]).Returns("60");

            _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsToken()
        {
            var username = "testuser";
            var password = "password";
            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "User"
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username)).ReturnsAsync(user);

            var token = await _authService.AuthenticateAsync(username, password);

            Assert.NotNull(token);
        }

        [Fact]
        public async Task AuthenticateAsync_InvalidPassword_ThrowsAuthenticationException()
        {
            var username = "testuser";
            var password = "wrongpassword";
            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                Role = "User"
            };

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username)).ReturnsAsync(user);

            var exception = await Assert.ThrowsAsync<AuthenticationException>(() => _authService.AuthenticateAsync(username, password));

            Assert.Equal("Invalid username or password.", exception.Message);
        }

        [Fact]
        public async Task AuthenticateAsync_UserNotFound_ThrowsAuthenticationException()
        {
            var username = "nonexistentuser";
            var password = "password";

            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username)).ReturnsAsync((User)null);

            var exception = await Assert.ThrowsAsync<AuthenticationException>(() => _authService.AuthenticateAsync(username, password));

            Assert.Equal("Invalid username or password.", exception.Message);
        }

    }
}
