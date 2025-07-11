using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ColorExtractorApi.Models.DTOs;
using ColorExtractorApi.Services;
using System.Security.Claims;

namespace ColorExtractorApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Update user info
        [HttpPut("me")] // PUT api/user/me

        public async Task<IActionResult> UpdateMyInfo([FromBody] UserUpdateRequestDto dto)
        {
            int userId = GetUserId();

            try
            {
                bool updated = await _userService.UpdateUserInfoAsync(userId, dto.Name, dto.Surname, dto.Email);
                if (!updated)
                    return BadRequest(new { message = "No changes detected or user not found." });

                return Ok(new { message = "User info updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                // e.g. email already in use
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating user info." });
            }
        }

        // Change password
        [HttpDelete("me")] // DELETE api/users/me
        public async Task<IActionResult> DeleteMyAccount([FromBody] UserDeleteRequestDto dto)
        {
            int userId = GetUserId();

            try
            {
                bool deleted = await _userService.DeleteUserAsync(userId, dto.Password);
                if (!deleted)
                    return NotFound(new { message = "User not found." });

                return Ok(new { message = "User account deleted successfully." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Incorrect password." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting user." });
            }
        }

        // Helper: Get logged-in user ID from JWT claims
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID claim not found.");

            return int.Parse(userIdClaim.Value);
        }
    }
}