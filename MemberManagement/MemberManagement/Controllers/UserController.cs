using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Project5.DTOs;
using Project5.Models;
using Project5.Services.Abstraction;
using System.Security.Claims;

namespace Project5.Controllers
{
    [ApiController]
    [Route("api/[Controller]/")]
    public class UserController:ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("/view-profile")]
        [Authorize]
        public async Task<IActionResult> GetUserData()
        {
            try
            {
                var resp = await userService.GetUserData();
                if (resp == null)
                {
                    return BadRequest(new {message="Error while fetching user data!"});
                }
                return Ok(resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return BadRequest(new { message = "Error while fetching user data!" });
            }
        }

        [HttpPut("/update-user")]
        [Authorize]
        public async Task<IActionResult> UpdateUserData([FromBody] UpdateMemberDTO updateUser)
        {
            try
            {
                var response =await userService.updateUserDetailsAsync(updateUser);
                if (response == null)
                {
                    return BadRequest(new ApiResponse() { Message = "Update Failed" });
                }
                else
                {
                    return Ok(new ApiResponse() { Message = "Profile Updated!" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return BadRequest(new ApiResponse() { Message = "Update Failed" });

            }


        }

        [HttpPost("/recover-password")]
        [AllowAnonymous]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordDTO recoverPassword)
        {
            try
            {
                var res = await userService.RecoverPasswordAsync(recoverPassword);
                if (res)
                {
                    return Ok(new ApiResponse{ Message = "Password is sent to your email."});
                }
                return BadRequest(new ApiResponse{ Message = "User not found" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.InnerException);
                return BadRequest(new ApiResponse { Message = "Failed to send password. Please try again." });
            }
        }

        [HttpPut("/reset-password")]
        [Authorize]
        public async Task<IActionResult> ResetPassword([FromBody] ChangeUserCredentialDTO changeUserCredential)
        {
            try
            {
                var response = await userService.ResetPasswordAsync(changeUserCredential);
                if (response)
                {
                    return Ok(new ApiResponse {Message = "Password was reset successfully!"});
                }
                return BadRequest(new ApiResponse{ Message = "Password reset failed. Please try again!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.InnerException);
                return BadRequest(new ApiResponse{  Message = "Password reset failed. Please try again!" });
            }
        }
        
    }
}
