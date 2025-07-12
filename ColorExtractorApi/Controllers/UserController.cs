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
                    return BadRequest(new { message = "No changes detected." });

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

        // Update user password
        [HttpPost("update-password")] // POST api/user/update-password
        public async Task<IActionResult> UpdatePassword([FromBody] UserUpdatePasswordRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match." });

            int userId = GetUserId();

            try
            {
                var result = await _userService.UpdatePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
                if (!result)
                    return NotFound(new { message = "User not found." });

                return Ok(new { message = "Password updated successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", detail = ex.Message });
            }
        }

        // Delete user account
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