using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PitStopAutoShop.Web.Helpers;
using PitStopAutoShop.Web.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Vereyon.Web;

namespace PitStopAutoShop.Web.Controllers
{
    public class MailboxController : Controller
    {
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;

        public MailboxController(IMailHelper mailHelper, IFlashMessage flashMessage)
        {
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
        }

        public IActionResult Announcement()
        {

            var model = new AnnouncementViewModel
            {
                To = _mailHelper.Destinations()
            };      

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Announcement(AnnouncementViewModel model)
        {

            if (ModelState.IsValid)
            {
                var path = "";

                if (model.Attachment != null)
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\tempData\" + model.Attachment.FileName);

                    using (var stream = System.IO.File.Create(path))
                    {
                        await model.Attachment.CopyToAsync(stream);
                    }
                }

                var response = await _mailHelper.SendAnnouncementAsync(model.ToId, model.Subject, model.Message, path);

                if (!string.IsNullOrEmpty(path))
                {
                    System.IO.File.Delete(path);
                }

                if(response.IsSuccess == true)
                {
                    _flashMessage.Confirmation("The annoucment was successfuly sent!");
                    return RedirectToAction(nameof(Announcement));
                }
                else
                {
                    _flashMessage.Warning("There was an error sending the announcement!");
                    model.To = _mailHelper.Destinations();
                    return View(model);
                }                
            }

            model.To = _mailHelper.Destinations();

            return View(model);
        }

    }
}
