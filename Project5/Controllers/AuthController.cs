using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project5.DTOs;
using Project5.Models;
using Project5.Services.Abstraction;
using Project5.Services.Implementation;

namespace Project5.Controllers
{
    [ApiController]
    [Route("api/[Controller]/")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;

        public AuthController(IUserService userService)
        {
            this.userService = userService;
        }
        //public AuthController() { }
        
        [HttpPost("/register-user")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserDTO registerUser)
        {
            try
            {
                var result = await userService.RegisterUserAsync(registerUser);
                if (!result)
                {
                    return BadRequest("Registration failed!");
                }
                else
                {
                    return Ok(new ApiResponse { Message = "User registered" });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return BadRequest("Registration failed!");

            }

        }

        [HttpPost("/login-user")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUserAsync([FromBody] LoginUserDTO loginUser)
        {
            try
            {
                var res = await userService.LoginUserAsync(loginUser);
                if (res == null)
                {
                    return Unauthorized("User is not authorized");
                }
                return Ok(new ApiResponse{ Token = res });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return Ok("User authorized");
        }

        [HttpDelete("/logout-user")]
        public async Task<IActionResult> LogoutUserAsync()
        {
            try
            {
               var resp = await userService.LogoutAsync();
                if (!resp)
                {
                    return BadRequest(new ApiResponse { Message = "Please try again" });
                }
                return Ok(new ApiResponse { Message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return BadRequest(new ApiResponse { Message = "Please try again" });
            }
        }
    }
}
