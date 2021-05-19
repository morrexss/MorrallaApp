using System.ComponentModel.DataAnnotations;

namespace MorrallaExpress.Validations
{
    public class EmailRule : IValidationRule<string>
    {
        public EmailRule() => ValidationMessage = "El correo no cumple con el formato: test@mail.com";

        public string ValidationMessage { get; set; }

        public bool Check(string value) => new EmailAddressAttribute().IsValid(value?.Trim());
    }
}
