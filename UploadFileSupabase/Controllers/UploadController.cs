using Microsoft.AspNetCore.Mvc;

namespace UploadFileSupabase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SupabaseUploadController : ControllerBase
    {
        [HttpPost]
        [Route("upload-from-url")]
        public async Task<IActionResult> UploadFromUrl([FromBody] UploadFromUrlRequest request)
        {
            var _uploader = new SupabaseUploader(request.UrlSupabase, request.AnonKeySupabase);
            
            var result = await _uploader.UploadFileFromUrlAsync(request.FileUrl, request.PathInBucket, request.BucketName);
            if (result.success)
            {
                return Ok(new { Url = result.url });
            }
            return BadRequest(new { Error = result.errorMessage });
        }

        [HttpPost]
        [Route("upload-file")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest request)
        {
            if (request.File.Length > 0)
            {
                // Usar o nome original do arquivo
                var _uploader = new SupabaseUploader(request.UrlSupabase, request.AnonKeySupabase);
                var originalFileName = Path.GetFileName(request.File.FileName);
                var filePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(filePath))
                {
                    await request.File.CopyToAsync(stream);
                }

                var result = await _uploader.UploadFileAsync(filePath, request.PathInBucket, request.BucketName, originalFileName);

                System.IO.File.Delete(filePath);

                if (result.success)
                {
                    return Ok(new { Url = result.url });
                }
                return BadRequest(new { Error = result.errorMessage });
            }
            return BadRequest(new { Error = "File is empty" });
        }
    }

    public class UploadFromUrlRequest
    {
        public required string UrlSupabase { get; set; }
        public required string AnonKeySupabase { get; set; }
        public required string FileUrl { get; set; }
        public required string PathInBucket { get; set; }
        public required string BucketName { get; set; }
    }

    public class UploadFileRequest
    {
        public required string UrlSupabase { get; set; }
        public required string AnonKeySupabase { get; set; }
        public required IFormFile File { get; set; }
        public required string PathInBucket { get; set; }
        public required string BucketName { get; set; }
    }
}
