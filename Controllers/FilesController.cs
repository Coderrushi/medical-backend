using medical_record_system_backend.Services;
using medical_record_system_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.StaticFiles;
using Azure;

namespace medical_record_system_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class FilesController : ControllerBase
    {
        private readonly IFileService _files;
        private readonly IWebHostEnvironment _env;

        public FilesController(IFileService files, IWebHostEnvironment env)
        {
            _files = files;
            _env = env;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(
                [FromForm] IFormFile file,
                [FromForm] string fileType,
                [FromForm] string displayName)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

            if (string.IsNullOrEmpty(fileType))
                return BadRequest("File type is required.");

            if (string.IsNullOrEmpty(displayName))
                return BadRequest("Display name is required.");

           
            string contentType = file.ContentType;

            
            var result = await _files.SaveFileAsync(
                file,
                CurrentUserId,
                fileType,
                displayName,
                contentType 
            );

            return Ok(result);
        }


       
        [HttpGet]
        public async Task<IActionResult> ListFiles()
        {
            var files = await _files.GetUserFilesAsync(CurrentUserId);
            return Ok(files);
        }

        
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var dbFile = await _files.GetFileByIdAsync(id);
            if (dbFile == null || dbFile.UserId != CurrentUserId)
                return NotFound(new { message = "File not found or unauthorized" });

            var fullPath = Path.Combine(_env.WebRootPath, dbFile.FilePath);

            if (!System.IO.File.Exists(fullPath))
                return NotFound(new { message = "File missing on server" });

            var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);

            
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(dbFile.FileName, out var mime))
                mime = "application/octet-stream";

            var downloadName = dbFile.DisplayName;
            if (!downloadName.Contains("."))
            {
                
                downloadName += Path.GetExtension(dbFile.FileName);
            }

          
            var safeName = Uri.EscapeDataString(downloadName);

            return File(bytes, mime, safeName);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _files.DeleteFileAsync(id, CurrentUserId);
            if (!success) return NotFound(new { message = "File not found or not authorized" });

            return Ok(new { message = "File deleted successfully" });
        }


        [HttpGet("preview/{id}")]
        public async Task<IActionResult> Preview(int id)
        {
            var dbFile = await _files.GetFileByIdAsync(id);
            if (dbFile == null || dbFile.UserId != CurrentUserId) return NotFound();

            var fullPath = Path.Combine(_env.WebRootPath, dbFile.FilePath);
            if (!System.IO.File.Exists(fullPath)) return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(dbFile.FileName, out var mime))
                mime = "application/octet-stream";

            
            Response.Headers["Content-Disposition"] = $"inline; filename=\"{dbFile.FileName}\"";
            return File(bytes, mime);
        }

    }
}
