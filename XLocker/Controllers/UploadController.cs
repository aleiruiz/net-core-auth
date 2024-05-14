using Microsoft.AspNetCore.Mvc;
using XLocker.Helpers;
using XLocker.Services;

namespace Boxtoken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController
    {
        private readonly IStorageService _storageService;

        public UploadController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost]
        public async Task<string> UploadFile([FromForm] IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                var ext = Path.GetExtension(file.FileName);
                await file.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return await _storageService.UploadFileFromMemoryStream(ms, $"{GenerateName.New(20)}{ext}");
            }

        }
    }
}
