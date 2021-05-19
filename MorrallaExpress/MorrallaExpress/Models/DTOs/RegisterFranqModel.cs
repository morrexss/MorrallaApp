using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Models.DTOs
{
    public class RegisterFranqRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string LastName2 { get; set; }
        public string Phone { get; set; }
        public bool HasVehicle { get; set; }
    }
}
