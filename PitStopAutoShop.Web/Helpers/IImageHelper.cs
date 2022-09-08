using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
    }
}
