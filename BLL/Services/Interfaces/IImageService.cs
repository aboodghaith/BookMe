using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IImageService
    {

        Task<string> UploadImageAsync(IFormFile imageFile, string directory);


    }
}
