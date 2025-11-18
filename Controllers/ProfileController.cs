using medical_record_system_backend.DTOs;
using medical_record_system_backend.Models;
using medical_record_system_backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace medical_record_system_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserRepository _users;

        public ProfileController(IUserRepository users)
        {
            _users = users;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _users.GetByIdAsync(CurrentUserId);
            if (user == null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Phone,
                user.Gender,
                AvatarUrl = string.IsNullOrEmpty(user.AvatarPath)
                    ? null
                    : $"{Request.Scheme}://{Request.Host}/{user.AvatarPath.Replace("wwwroot/", "")}"
            });
        }

       
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req)
        {
            if (req == null)
                return BadRequest("No data received");

            var user = await _users.GetByIdAsync(CurrentUserId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            if (!string.IsNullOrWhiteSpace(req.FullName))
                user.FullName = req.FullName;

            if (!string.IsNullOrWhiteSpace(req.Phone))
                user.Phone = req.Phone;

            if (!string.IsNullOrWhiteSpace(req.Gender))
                user.Gender = req.Gender;

            await _users.UpdateAsync(user);

            return Ok(new { message = "Profile updated successfully!" });
        }



        
        [HttpPost("avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            var user = await _users.GetByIdAsync(CurrentUserId);
            if (user == null) return NotFound();

           
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "Max allowed size is 5MB" });

            
            var allowed = new[] { "image/jpeg", "image/png", "image/jpg" };
            if (!allowed.Contains(file.ContentType))
                return BadRequest(new { message = "Only JPG and PNG files are allowed" });

            var avatarFolder = Path.Combine("wwwroot", "avatars", user.Id.ToString());
            if (!Directory.Exists(avatarFolder))
                Directory.CreateDirectory(avatarFolder);

            
            if (!string.IsNullOrEmpty(user.AvatarPath))
            {
                var oldFile = Path.Combine("wwwroot", user.AvatarPath);
                if (System.IO.File.Exists(oldFile))
                    System.IO.File.Delete(oldFile);
            }

            
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(avatarFolder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            
            user.AvatarPath = $"avatars/{user.Id}/{fileName}";
            await _users.UpdateAsync(user);

            return Ok(new
            {
                message = "Avatar uploaded successfully",
                avatarUrl = $"{Request.Scheme}://{Request.Host}/{user.AvatarPath}"
            });
        }


       
        [HttpDelete("avatar")]
        public async Task<IActionResult> DeleteAvatar()
        {
            var user = await _users.GetByIdAsync(CurrentUserId);
            if (user == null) return NotFound();

            if (string.IsNullOrEmpty(user.AvatarPath))
                return BadRequest(new { message = "No avatar found" });

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), user.AvatarPath);

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);

            user.AvatarPath = null;
            await _users.UpdateAsync(user);

            return Ok(new { message = "Avatar deleted successfully" });
        }
    }
}
