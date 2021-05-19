using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Models.DTOs
{
    public class RegisterClientRequestModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        public string MobilePhone { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string CompanyName { get; set; }
        public string Rfc { get; set; }

        public int UsoId { get; set; } //si sirve
        public int CityId { get; set; } // si sirve

        public string FiscalName { get; set; }
        public string FiscalAddress { get; set; }
        public string FiscalCity { get; set; }
        public string FiscalState { get; set; }
        public string FiscalPostalCode { get; set; }
        //public string FiscalDelegationName { get; set; }
        public string FireBaseToken { get; set; }
    }
}
