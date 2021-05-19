using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Validations
{
    public class ComparePasswordsRule : IComparisonRule<string>
    {
        public ComparePasswordsRule() => ValidationMessage = "Passwords do not match";

        public string ValidationMessage { get; set; }
        public ValidatableObject<string> ValueToCompare { get; set; }

        public bool Compare(string value) => value == ValueToCompare.Value;
    }
}
