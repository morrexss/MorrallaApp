using MorrallaExpress.Models.DTOs;
using System;
namespace MorrallaExpress.Validations.Rules
{
    public class IsNotNullOrEmptyRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value == null)
                return false;
            if (value is UsoCFDIModel uso)
                return !string.IsNullOrEmpty(uso.Name);
            if (value is StateModel state)
                return !string.IsNullOrEmpty(state.Name);
            if (value is CityModel city)
                return !string.IsNullOrEmpty(city.Name);
            var str = value as string;
            return !string.IsNullOrWhiteSpace(str);
        }
    }
}
