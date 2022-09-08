using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace PitStopAutoShop.Web.Helpers
{
    public interface IBlobHelper
    {
        Task<Guid> UploadBlobAsync(IFormFile file, String containerName);

        Task<Guid> UploadBlobAsync(byte[] file, String containerName);

        Task<Guid> UploadBlobAsync(string image, String containerName);
    }
}
