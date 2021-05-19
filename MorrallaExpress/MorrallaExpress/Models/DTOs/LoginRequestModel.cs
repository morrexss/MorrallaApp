using Plugin.FirebasePushNotification;
using System;
namespace MorrallaExpress.Models.DTOs
{
    public class LoginRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get => CrossFirebasePushNotification.Current.Token; }
    }
}
