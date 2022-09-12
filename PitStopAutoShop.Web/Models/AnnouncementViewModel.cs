﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PitStopAutoShop.Web.Models
{
    public class AnnouncementViewModel
    {
        public IEnumerable<SelectListItem> To { get; set; }

        [Required(ErrorMessage = "Must insert the {0} Destination.")]
        [Display(Name = "To:")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a To Destination")]
        public int ToId { get; set; }

        [Required(ErrorMessage ="Must insert the {0}.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Must insert the {0}.")]
        public string Message { get; set; }

        public IFormFile Attachment { get; set; }

    }
}
