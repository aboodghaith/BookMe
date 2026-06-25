using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class ImageService : IImageService
    {
        public async Task<string> UploadImageAsync(IFormFile imageFile, string directory)
        {
            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", directory);

            if (!Directory.Exists(uploadDirectory))
                Directory.CreateDirectory(uploadDirectory);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadDirectory, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await imageFile.CopyToAsync(stream);

            return $"/{directory}/{fileName}";
        }
    }
}
