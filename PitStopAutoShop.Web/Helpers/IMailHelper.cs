
using System.Collections.Generic;

namespace PitStopAutoShop.Web.Helpers
{
    public interface IMailHelper
    {
        Response SendEmail(string to, string subject, string body,string attachment);   


    }
}
