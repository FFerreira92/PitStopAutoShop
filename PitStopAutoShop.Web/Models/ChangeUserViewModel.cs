using Microsoft.AspNetCore.Http;
using PitStopAutoShop.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PitStopAutoShop.Web.Models
{
    public class ChangeUserViewModel : User
    {
        [Display(Name = "Tax Identification Number / NIF")]
        public string Nif { get; set; }  

        public bool HasPassword { get; set; }

    }
}
