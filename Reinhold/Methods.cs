using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Reinhold
{
    class Methods
    {
        public string CallNumber(string Number)
        {
            try
            {
                PhoneDialer.Open(Number);
            }
            catch (Exception ex)
            {
                return "I am sorry, something went wrong. I am unable to call this number.";
            }
            return $"Calling {Number}.";
        }
        public async Task<string> CallContact(string Contact)
        {
            try
            {
                PhoneDialer.Open(Number);
            }
            catch (Exception ex)
            {
                return "I am sorry, something went wrong. I am unable to call this number.";
            }
            return $"Calling {Number}.";
        }

        public string BatteryLevel()
        {
            return $"Battery charge is {Battery.ChargeLevel * 100}%";
        }

        public string Time()
        {
            return $"It is {DateTime.Now.ToString("HH:MM:ss")}";
        }
        public string Date()
        {
            return $"Today's date is {DateTime.Now.ToString("yyyy/MM/dd")}";
        }
    }
}
