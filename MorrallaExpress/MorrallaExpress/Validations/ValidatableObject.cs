using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Prism.Mvvm;

namespace MorrallaExpress.Validations
{
    public class ValidatableObject<T> : BindableBase, IValidity
    {
        T mainValue;
        bool isValid;

        public List<IValidationRule<T>> Validations { get; }
        public List<IComparisonRule<T>> Comparison { get; }

        public ObservableCollection<string> Errors { get; }

        private string _ErrorMessage;
        public string ErrorMessage { get => _ErrorMessage; set => SetProperty(ref _ErrorMessage, value); }

        public T Value
        {
            get => mainValue;

            set
            {
                SetProperty(ref mainValue, value);
            }
        }

        public bool IsValid
        {
            get => isValid;

            set
            {
                SetProperty(ref isValid, value);
            }
        }

        public ValidatableObject()
        {
            isValid = true;
            Errors = new ObservableCollection<string>();
            Validations = new List<IValidationRule<T>>();
            Comparison = new List<IComparisonRule<T>>();
        }

        public bool Validate()
        {
            Errors.Clear();
            IsValid = true;
            var errors = Validations.Where(v => !v.Check(Value))
                .Select(v => v.ValidationMessage).ToList();
            errors.AddRange(Comparison.Where(x => !x.Compare(Value)).Select(v => v.ValidationMessage));

            foreach (var error in errors)
            {
                Errors.Add(error);
            }

            if (Errors.Any())
            {
                IsValid = false;
                ErrorMessage = Errors.First();
            }
            return IsValid;
        }
    }
}
