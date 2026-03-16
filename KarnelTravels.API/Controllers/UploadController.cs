using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KarnelTravels.API.DTOs;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public UploadController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Upload single image for admin use (Tourist Spots, Tours, Hotels, etc.)
    /// </summary>
    [HttpPost("image")]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = 10485760)]
    public async Task<ActionResult<ApiResponse<UploadResult>>> UploadImage()
    {
        try
        {
            var form = await Request.ReadFormAsync();
            var file = form.Files.FirstOrDefault();

            if (file == null || file.Length == 0)
            {
                return BadRequest(new ApiResponse<UploadResult>
                {
                    Success = false,
                    Message = "Please select an image file"
                });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new ApiResponse<UploadResult>
                {
                    Success = false,
                    Message = $"Only image files are allowed. Got: {extension}"
                });
            }

            if (file.Length > 10 * 1024 * 1024)
            {
                return BadRequest(new ApiResponse<UploadResult>
                {
                    Success = false,
                    Message = "Image size cannot exceed 10MB"
                });
            }

            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"/uploads/images/{fileName}";

            return Ok(new ApiResponse<UploadResult>
            {
                Success = true,
                Message = "Image uploaded successfully",
                Data = new UploadResult
                {
                    Url = url,
                    FileName = fileName
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<UploadResult>
            {
                Success = false,
                Message = $"Server error: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Upload multiple images for admin use
    /// </summary>
    [HttpPost("images")]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = 10485760)]
    public async Task<ActionResult<ApiResponse<List<UploadResult>>>> UploadImages()
    {
        try
        {
            var form = await Request.ReadFormAsync();
            var files = form.Files;
            if (files == null || files.Count == 0)
            {
                return BadRequest(new ApiResponse<List<UploadResult>>
                {
                    Success = false,
                    Message = "Please select image files"
                });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "images");
            Directory.CreateDirectory(uploadsFolder);

            var results = new List<UploadResult>();

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    continue;
                }

                if (file.Length > 10 * 1024 * 1024)
                {
                    continue;
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                results.Add(new UploadResult
                {
                    Url = $"/uploads/images/{fileName}",
                    FileName = fileName
                });
            }

            return Ok(new ApiResponse<List<UploadResult>>
            {
                Success = true,
                Message = $"Uploaded {results.Count} images successfully",
                Data = results
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<UploadResult>>
            {
                Success = false,
                Message = $"Server error: {ex.Message}"
            });
        }
    }
}

public class UploadResult
{
    public string Url { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}
