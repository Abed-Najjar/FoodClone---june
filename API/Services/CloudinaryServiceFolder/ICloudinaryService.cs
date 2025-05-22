using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace API.Services.CmsServiceFolder
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "uploads");
    }
}
