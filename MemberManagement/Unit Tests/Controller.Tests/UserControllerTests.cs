using Microsoft.AspNetCore.Mvc;
using Moq;
using Project5.Controllers;
using Project5.DTOs;
using Project5.DTOs.Project5.DTOs;
using Project5.Models;
using Project5.Services.Abstraction;
using Xunit;

public class UserControllerTests
{
    private readonly Mock<IUserService> mockUserService;
    private readonly UserController userController;

    public UserControllerTests()
    {
        mockUserService = new Mock<IUserService>();
        userController = new UserController(mockUserService.Object);
    }

    [Fact]
    public async Task UpdateUserData_ReturnsOk_WhenUserDataIsUpdatedSuccessfully()
    {
        // Arrange
        var updateMemberDto = new UpdateMemberDTO
        {
            FirstName = "John",
            MiddleName = "A.",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 15),
            Gender = "Male"
        };
        mockUserService.Setup(service => service.updateUserDetailsAsync(updateMemberDto)).ReturnsAsync(new Member()
        {
            FirstName = "John",
            MiddleName = "A.",
            LastName = "Doe",
            Dob = new DateTime(1990, 1, 15),
            Gender = "Male"
        });

        // Act
        var result = await userController.UpdateUserData(updateMemberDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateUserData_ReturnsBadRequest_WhenUpdateFails()
    {
        // Arrange
        var updateMemberDto = new UpdateMemberDTO
        {

        };

        mockUserService.Setup(service => service.updateUserDetailsAsync(updateMemberDto)).ReturnsAsync((Member)null);


        var result = await userController.UpdateUserData(updateMemberDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var resp = Assert.IsType<ApiResponse>(badRequestResult.Value);
        Assert.Equal("Update Failed", resp.Message);
    }

    [Fact]
    public async Task RecoverPassword_ReturnsOk_WhenPasswordIsSentSuccessfully()
    {
        // Arrange
        var recoverPasswordDto = new RecoverPasswordDTO { Email = "john.doe@example.com" };
        mockUserService.Setup(service => service.RecoverPasswordAsync(recoverPasswordDto)).ReturnsAsync(true);

        // Act
        var result = await userController.RecoverPassword(recoverPasswordDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse>(okResult.Value); // Anonymous object
        Assert.Equal("Password is sent to your email.", response.Message);
    }

    [Fact]
    public async Task RecoverPassword_ReturnsBadRequest_WhenUserNotFound()
    {
        var recoverPasswordDto = new RecoverPasswordDTO { Email = "john.doe@example.com" };
        mockUserService.Setup(service => service.RecoverPasswordAsync(recoverPasswordDto)).ReturnsAsync(false);

        
        var result = await userController.RecoverPassword(recoverPasswordDto);


        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var resp = Assert.IsType<ApiResponse>(badRequestResult.Value);
        Assert.Equal("User not found", resp.Message);
    }

    [Fact]
    public async Task ResetPassword_ReturnsOk_WhenPasswordIsResetSuccessfully()
    {
        // Arrange
        var resetPasswordDto = new ChangeUserCredentialDTO { OldPassword = "newpassword", NewPassword = "newpassword" };
        mockUserService.Setup(service => service.ResetPasswordAsync(resetPasswordDto)).ReturnsAsync(true);

        // Act
        var result = await userController.ResetPassword(resetPasswordDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resp = Assert.IsType<ApiResponse>(okResult.Value);

        Assert.Equal("Password was reset successfully!", resp.Message);
    }

    [Fact]
    public async Task ResetPassword_ReturnsBadRequest_WhenPasswordResetFails()
    {
        // Arrange
        var resetPasswordDto = new ChangeUserCredentialDTO { OldPassword = "newpassword", NewPassword="newpassword" };
        mockUserService.Setup(service => service.ResetPasswordAsync(resetPasswordDto)).ReturnsAsync(false);

        // Act
        var result = await userController.ResetPassword(resetPasswordDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var resp = Assert.IsType<ApiResponse>(badRequestResult.Value);
        Assert.Equal("Password reset failed. Please try again!", resp.Message);
    }
}
