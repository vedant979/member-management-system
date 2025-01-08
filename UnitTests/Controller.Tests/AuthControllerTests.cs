using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Project5.Controllers;
using Project5.DTOs;
using Project5.Models;
using Project5.Services.Abstraction;
using Project5.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnitTests.Controller.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly AuthController _controller;

        // Constructor sets up the mock and the controller
        public AuthControllerTests()
        {
            _mockUserService = new Mock<IUserService>();  // Mocking IUserService directly
            _controller = new AuthController(_mockUserService.Object);  // Injecting the mock into the controller
        }

        [Fact]
        public async Task RegisterUserAsync_ValidUser_ReturnsTrue()
        {
            // Arrange
            RegisterUserDTO validUser = new RegisterUserDTO()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "SecurePassw0rd123"
            };

            var res = _mockUserService
           .Setup(service => service.RegisterUserAsync(It.IsAny<RegisterUserDTO>()))
           .ReturnsAsync(true);
            var result = await _controller.RegisterUserAsync(validUser);

            var okResult = Assert.IsType<OkObjectResult>(result); 
            Assert.Equal(200, okResult.StatusCode); 
        }
        [Fact]
        public async Task RegisterUserAsync_ReturnsBadRequest_WhenRegistrationFails()
        {
            
            var userDto = new RegisterUserDTO 
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.do123e@example.com",
                Password = "SecurePassw0rd123"
            };
            _mockUserService.Setup(service => service.RegisterUserAsync(It.IsAny<RegisterUserDTO>()))
                .ReturnsAsync(false);  

            // Act: Call the method under test
            var result = await _controller.RegisterUserAsync(userDto);

            // Assert: Verify the result type
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Registration failed!", badRequestResult.Value);
        }
        [Fact]
        public async Task RegisterUserAsync_ReturnsBadRequest_WhenExceptionOccurs()
        {

            var userDto = new RegisterUserDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.do123e@example.com",
                Password = "SecurePassw0rd123"
            };
            _mockUserService.Setup(service => service.RegisterUserAsync(It.IsAny<RegisterUserDTO>()))
                .ThrowsAsync(new Exception("Some error occurred")); 

            // Act: Call the method under test
            var result = await _controller.RegisterUserAsync(userDto);

            // Assert: Verify the result type
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Registration failed!", badRequestResult.Value);
        }
        [Fact]
        public async Task LoginUserAsync_ReturnsUnauthorized_WhenInvalidCredentials()
        {
            // Arrange: Set up mock to return null (or any invalid result) for LoginUserAsync
            var loginUserDTO = new LoginUserDTO { Email = "test@example.com", Password = "wrongpassword" };
            _mockUserService.Setup(service => service.LoginUserAsync(It.IsAny<LoginUserDTO>()))
                .ReturnsAsync((string)null);  // Simulate login failure

            // Act: Call the controller method
            var result = await _controller.LoginUserAsync(loginUserDTO);

            // Assert: Verify that the result is Unauthorized
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User is not authorized", unauthorizedResult.Value);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsOk_WhenLoginIsSuccessful()
        {
            var loginUserDTO = new LoginUserDTO { Email = "test@example.com", Password = "validpassword" };
            var mockToken = "ValidToken123";
            _mockUserService.Setup(service => service.LoginUserAsync(It.IsAny<LoginUserDTO>()))
                .ReturnsAsync(mockToken);

            var result = await _controller.LoginUserAsync(loginUserDTO);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.NotNull(response);
            Assert.Equal(mockToken, response.Token);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsOk_WhenExceptionOccurs()
        {
            // Arrange: Set up mock to throw an exception
            var loginUserDTO = new LoginUserDTO { Email = "test@example.com", Password = "validpassword" };
            _mockUserService.Setup(service => service.LoginUserAsync(It.IsAny<LoginUserDTO>()))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act: Call the controller method
            var result = await _controller.LoginUserAsync(loginUserDTO);

            // Assert: Verify that the result is Ok with a generic "User authorized" message
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User authorized", okResult.Value);
        }

        [Fact]
        public async Task LogoutUserAsync_ReturnsOk_WhenLogoutIsSuccessful()
        {
            // Arrange
            _mockUserService.Setup(service => service.LogoutAsync())
                .ReturnsAsync(true); // Simulate a successful logout

            // Act
            var result = await _controller.LogoutUserAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // Ensure response is OkObjectResult
            var response = Assert.IsType<ApiResponse>(okResult.Value);

            Assert.NotNull(response);
            Assert.Equal("Logged out successfully", response.Message);
        }

        [Fact]
        public async Task LogoutUserAsync_ReturnsBadRequest_WhenLogoutFails()
        {
            // Arrange
            _mockUserService.Setup(service => service.LogoutAsync())
                .ReturnsAsync(false); // Simulate a failed logout

            var result = await _controller.LogoutUserAsync();
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.Equal("Please try again", response.Message);

        }

        [Fact]
        public async Task LogoutUserAsync_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            _mockUserService.Setup(service => service.LogoutAsync())
                .ThrowsAsync(new Exception("Service error")); // Simulate an exception during logout

            // Act
            var result = await _controller.LogoutUserAsync();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result); // Ensure response is BadRequestObjectResult
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.NotNull(response);
            Assert.Equal("Please try again", response.Message);
        }

    }
}

