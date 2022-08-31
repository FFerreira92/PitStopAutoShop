using System;
using System.ComponentModel.DataAnnotations;

namespace PitStopAutoShop.Web.Data.CustomValidation
{
    public class DateAfterCurrentTimeValidator : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateTime = Convert.ToDateTime(value);
            return dateTime > DateTime.Now;
        }

    }
}
