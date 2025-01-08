using Microsoft.AspNetCore.Mvc;
using Moq;
using Project5.Controllers;
using Project5.DTOs;
using Project5.Models;
using Project5.Services.Abstraction;
using Xunit;

public class ContactControllerTests
{
    private readonly Mock<IUserService> mockUserService;
    private readonly ContactController contactController;

    public ContactControllerTests()
    {
        mockUserService = new Mock<IUserService>();
        contactController = new ContactController(mockUserService.Object);
    }

    [Fact]
    public async Task AddContact_ReturnsOk_WhenContactIsAddedSuccessfully()
    {
        // Arrange
        var addContactDto = new AddContactDTO
        {
            PhoneNumber = 1111111111
        };

        mockUserService
            .Setup(service => service.AddContactAsync(It.IsAny<AddContactDTO>()))
            .ReturnsAsync(true);

        // Act
        var result = await contactController.AddContact(addContactDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse>(okResult.Value);
        Assert.Equal("Contact added successfully", response.Message);
    }

    [Fact]
    public async Task AddContact_ReturnsBadRequest_WhenContactAdditionFails()
    {
        // Arrange
        var addContactDto = new AddContactDTO
        {
            PhoneNumber = 1111111111
        };

        mockUserService
            .Setup(service => service.AddContactAsync(It.IsAny<AddContactDTO>()))
            .ReturnsAsync(false);

        // Act
        var result = await contactController.AddContact(addContactDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse>(badRequestResult.Value);
        Assert.Equal("There was an error", response.Message);
    }

    [Fact]
    public async Task UpdateContactData_ReturnsOk_WhenContactIsUpdatedSuccessfully()
    {
        // Arrange
        var updateContactDto = new UpdateContactDTO
        {
            OldPhoneNumber = 1111111111,
            NewPhoneNumber = 1111111111,
            ContactType = "current"
        };

        mockUserService
            .Setup(service => service.UpdateContactAsync(It.IsAny<UpdateContactDTO>()))
            .ReturnsAsync(true);


        var result = await contactController.UpdateContactData(updateContactDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse>(okResult.Value); 
        Assert.Equal("contact updated", response.Message);
    }

    [Fact]
    public async Task UpdateContactData_ReturnsBadRequest_WhenContactUpdateFails()
    {
        // Arrange
        var updateContactDto = new UpdateContactDTO
        {
            OldPhoneNumber = 1111111111,
            NewPhoneNumber = 1111111112,
            ContactType = "current"
        };

        mockUserService
            .Setup(service => service.UpdateContactAsync(It.IsAny<UpdateContactDTO>()))
            .ReturnsAsync(false);

        // Act
        var result = await contactController.UpdateContactData(updateContactDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse>(badRequestResult.Value);
        Assert.Equal("Try again", response.Message);
    }
}
