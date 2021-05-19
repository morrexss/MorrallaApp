using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace MorrallaExpress.Validations
{
    public class PasswordRule : IValidationRule<string>
    {
        public PasswordRule() => ValidationMessage = $"La contraseña debe contener mínimo 8 carácteres.";

        public string ValidationMessage { get; set; }

        public bool Check(string value) => ValidatePassword(value ?? "");

        public bool ValidatePassword(string value)
        {
            return value.Length >= 8;
        }
    }
}
