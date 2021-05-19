using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Models.DTOs
{
    public class PaymentMethodModel : BindableBase
    {
        public DelegateCommand<PaymentMethodModel> DeleteCommand { get; set; }
            = new DelegateCommand<PaymentMethodModel>(e => { });
        public int PaymentMethodId { get; set; }
        public string ApplicationUserId { get => UserId; }
        public string TokenId { get; set; }
        public string CardNumber { get; set; }
        public string CardId { get; set; }
        public string MaskedCard { get; set; }
        public string Expiration { get; set; }
        public string Cvv { get; set; }
        public string UserId { get; set; }
        public string CardType { get; set; }
        public string DeviceId { get; set; }
        public string HolderName { get; set; }
        public string CardIcn {
            get {
                switch (CardType)
                {
                    case "visa":
                        return "Visa_icn.png";
                    case "master_card":
                        return "MasterCard_icn.png";
                    case "american_express":
                        return "AmericanExp_icn.png";
                }
                return null;
            }
        }
    }
}
