using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Models.DTOs
{
    public class LoginResponseModel
    {
        public UserSession UserData { get; set; }
        public int Status { get; set; }
        public string Token { get; set; }
    }

    public class CheckEmailResponseModel
    {
        public string Exists { get; set; }
    }
}
