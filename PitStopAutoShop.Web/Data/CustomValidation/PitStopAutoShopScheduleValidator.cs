using System;
using System.ComponentModel.DataAnnotations;

namespace PitStopAutoShop.Web.Data.CustomValidation
{
    public class PitStopAutoShopScheduleValidator : ValidationAttribute
    {
        /// <summary>
        /// Check if the appointment is made during bussiness hours
        /// </summary>
        /// <param name="value">Datetime</param>
        /// <returns>True if the appointment is during bussiness hours</returns>

        public override bool IsValid(object value)
        {

            var shopOpenningHours = 9;

            var shopClosingHours = 18;

            var dateTime = Convert.ToDateTime(value);

            if(dateTime.DayOfWeek == DayOfWeek.Saturday)
            {
                shopClosingHours = 13;
            }

            if(dateTime.Hour < shopOpenningHours || dateTime.Hour > shopClosingHours)
            {
                return false;
            } 
            
            return true;
        }

    }
}
