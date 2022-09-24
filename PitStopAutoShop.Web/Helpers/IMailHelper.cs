
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PitStopAutoShop.Web.Helpers
{
    public interface IMailHelper
    {
        Task<Response> SendEmail(string to, string subject, string body,string attachment);

        List<SelectListItem> Destinations();

        Task<Response> SendAnnouncementAsync(int to, string subject, string body, string path);

        Task<Response> SendContactEmailAsync(string email, string subject, string message,string custName);
    }
}
