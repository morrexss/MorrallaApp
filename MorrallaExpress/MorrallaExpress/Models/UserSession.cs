using MorrallaExpress.Models.DTOs;
using Prism.Mvvm;
using System;
namespace MorrallaExpress.Models
{
    public class UserSession
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        public string HomePhone { get; set; }
        public string Rfc { get; set; }
        public string Curp { get; set; }
        public string Document1 { get; set; }
        public string Document2 { get; set; }
        public string[] Roles { get; set; }
        public int? CityId { get; set; }
        public string FiscalName { get; set; }
        public string FiscalAddress { get; set; }
        public string FiscalCity { get; set; }
        public string FiscalState { get; set; }
        public string FiscalPostalCode { get; set; }
        public string FiscalDelegation { get; set; }
        public string CompanyName { get; set; }
        public string FirebaseToken { get; set; }
        public string OpenPayToken { get; set; }
        public string OpenPayCustomerId { get; set; }
        public string Last4Digits { get; set; }
        public string ExpDate { get; set; }
        public string Photo { get; set; }
        public int DeliveryCount { get; set; }
        public decimal RatingAvg { get; set; }
        public string Bio { get; set; }
        public int? UsoId { get; set; }
        public string Uso { get; set; }
        public LocationModel[] Locations { get; set; } = new LocationModel[] { };
        public PaymentMethodModel[] PaymentMethods { get; set; } = new PaymentMethodModel[] { };
        public string BankName { get; set; }
        public string BankAccountHolderName { get; set; }
        public string BankAccountClabe { get; set; }
        public Rating Rating { get; set; }

        public DateTime DateCreated { get; set; }
    }

    public class Rating
    {
        public int Gold { get; set; }
        public int Silver { get; set; }
        public int Bronze { get; set; }
    }
}
