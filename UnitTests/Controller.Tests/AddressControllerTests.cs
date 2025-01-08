using Microsoft.AspNetCore.Mvc;
using Moq;
using Project5.Controllers;
using Project5.DTOs;
using Project5.Models;
using Project5.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Controller.Tests
{
    public class AddressControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly AddressController _controller;

        public AddressControllerTests()
        {
            _mockUserService = new Mock<IUserService>(); 
            _controller = new AddressController(_mockUserService.Object); 
        }

        [Fact]
        public async Task   AddAddressAsync_ReturnsOk()
        {

            AddressDTO address = new AddressDTO
            {
                Street = "123 Main Street",
                PinCode = "560001",
                City = "Bangalore",
                State = "Karnataka",
                Country = "India",
                Houseno = "A-12",
                AddressType = "current"
            };
            

            var res = _mockUserService
           .Setup(service => service.addAddressDetailsAsync(It.IsAny<AddressDTO>()))
           .ReturnsAsync(new Project5.Models.Address() { });

            var result = await _controller.AddAddress(address);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User address added!", okResult.Value);
        }
        [Fact]
        public async Task AddAddressAsync_ReturnsBadrequest()
        {

            AddressDTO address = new AddressDTO
            {
                Street = "123 Main Street",
                PinCode = "560001",
                City = "Bangalore",
                State = "Karnataka",
                Country = "India",
                Houseno = "A-12",
                AddressType = "current"
            };

            Address addressTest = null; 
            var res = _mockUserService
           .Setup(service => service.addAddressDetailsAsync(It.IsAny<AddressDTO>()))
           .ReturnsAsync(addressTest);

            var result = await _controller.AddAddress(address);

            var BadRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User address not added", BadRequest.Value);
        }
        [Fact]
        public async Task AddAddress_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var addressDTO = new AddressDTO
            {
                Street = "123 Main Street",
                PinCode = "560001",
                City = "Bangalore",
                State = "Karnataka",
                Country = "India",
                Houseno = "A-12",
                AddressType = "current"
            };

            var mockUserService = new Mock<IUserService>();
            mockUserService
                .Setup(service => service.addAddressDetailsAsync(addressDTO))
                .ThrowsAsync(new Exception("Database error")); // Mocking an exception

            //var controller = new YourController(mockUserService.Object);

            // Act
            var result = await _controller.AddAddress(addressDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User address not added", badRequestResult.Value);
        }

    }
}
