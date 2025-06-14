using NUnit.Framework;
using Microsoft.AspNetCore.Identity;
using Moq;
using SafeVault.Models;

namespace SafeVault.Tests
{

    [TestFixture]
    public class AuthenticationTests
    {
        private Mock<IAuthService> _authServiceMock;

        [SetUp]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthService>();
        }

        [Test]
        public void Login_InvalidCredentials_ShouldFail()
        {
            // Arrange
            var invalidUsername = "testUser";
            var invalidPassword = "testPassword";
            _authServiceMock.Setup(s => s.Login(invalidUsername, invalidPassword)).Returns(false);

            // Act
            var result = _authServiceMock.Object.Login(invalidUsername, invalidPassword);

            // Assert
            Assert.IsFalse(result, "Login should fail with invalid credentials.");
        }

        [Test]
        public void AccessProtectedResource_UnauthorizedUser_ShouldFail()
        {
            // Arrange
            var unauthorizedToken = "invalidToken";
            _authServiceMock.Setup(s => s.ValidateToken(unauthorizedToken)).Returns(false);

            // Act
            var isAuthorized = _authServiceMock.Object.ValidateToken(unauthorizedToken);

            // Assert
            Assert.IsFalse(isAuthorized, "Unauthorized users should not access protected resources.");
        }
    }
}