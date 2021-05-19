using Acr.UserDialogs;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models.DTOs;
using MorrallaExpress.Validations;
using MorrallaExpress.Validations.Rules;
using MorrallaExpress.Views.Login.Templates;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace MorrallaExpress.ViewModels.Login
{
    public class RegisterFranqPageViewModel : ViewModelBase
    {
        #region Commands
        public DelegateCommand RegisterCommand { get; set; }
        #endregion
        #region Props
       
        public List<DataTemplate> RegisterPageSelected { get; set; }
        public bool TermsChecked { get; set; }

        #region BasicInformation

        ValidatableObject<string> _email = new ValidatableObject<string>();
        public ValidatableObject<string> Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        ValidatableObject<string> _name = new ValidatableObject<string>();
        public ValidatableObject<string> Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        ValidatableObject<string> _lastName = new ValidatableObject<string>();
        public ValidatableObject<string> LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }
        ValidatableObject<string> _secondLastName = new ValidatableObject<string>();
        public ValidatableObject<string> SecondLastName
        {
            get => _secondLastName;
            set => SetProperty(ref _secondLastName, value);
        }
        ValidatableObject<string> _phoneNumber = new ValidatableObject<string>();
        public ValidatableObject<string> PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }
        ValidatableObject<string> _vehicle = new ValidatableObject<string>();
        public ValidatableObject<string> Vehicle
        {
            get => _vehicle;
            set => SetProperty(ref _vehicle, value);
        }
        #endregion

        #endregion
        public RegisterFranqPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            Title = "Registro Socio Morrexss";
            RegisterCommand = new DelegateCommand(Register);
            AddValidations();

            //RegisterPageSelected = new List<DataTemplate>
            //{
            //    new DataTemplate(() => { return new BasicClientInfoTemplate { ParentContext = this }; }),
            //    new DataTemplate(() => { return new FranqContactTemplate { ParentContext = this }; })
            //};
        }

        void NextStep() { }

        async void Register()
        {
            if (Validate())
            {
                //TODO: Register Actio
                var model = new RegisterFranqRequestModel {
                    Email = Email.Value,
                    Name = Name.Value,
                    LastName = LastName.Value,
                    LastName2 = SecondLastName.Value,
                    Phone = PhoneNumber.Value,
                    HasVehicle = Vehicle.Value == "Si"
                };
                string caractEspecial = @"[0-9]";
                bool resultado = Regex.IsMatch(Name.Value, caractEspecial, RegexOptions.IgnoreCase);
                bool resultado2 = Regex.IsMatch(LastName.Value, caractEspecial, RegexOptions.IgnoreCase);
                bool resultado3 = Regex.IsMatch(SecondLastName.Value, caractEspecial, RegexOptions.IgnoreCase);
                var res = false;
                using (UserDialogs.Instance.Loading("Cargando..."))
                    if (resultado)
                    {
                        await UserDialogs.Instance.AlertAsync("Inténtalo de nuevo. El campo de nombre deben ser letras.", "Error");
                        return;
                    }
                    else if (resultado2 || resultado3)
                    {
                        await UserDialogs.Instance.AlertAsync("Inténtalo de nuevo. El campo de apellidos deben ser letras. ", "Error");
                        return;
                    }
                res = await HttpService.RegisterFranq(model);
                if (res)
                {
                    await PopUp("¡Bienvenido a Morrexss!", "Tu registro se a realizado con éxito. \n Nos pondremos en contacto.");
                    await NavigateBack();
                }
            }
            else
                await PopUp("Te hacen falta datos", "Por favor revisa que todos los campos estén completos.");
        }

        #region FieldValidations
        bool Validate()
        {
            var isValidEmail = _email.Validate();
            Email.IsValid = isValidEmail;

            var isValidName = _name.Validate();
            Name.IsValid = isValidName;

            var isValidFLastname = _lastName.Validate();
            LastName.IsValid = isValidFLastname;

            var isValidSLastname = _secondLastName.Validate();
            SecondLastName.IsValid = isValidSLastname;

            var isValidPhone = _phoneNumber.Validate();
            PhoneNumber.IsValid = isValidPhone;

            var isValidVehicle = _vehicle.Validate();
            Vehicle.IsValid = isValidVehicle;

            return isValidEmail && isValidName && isValidFLastname && isValidSLastname && isValidPhone && isValidVehicle;
        }

        void AddValidations()
        {
            _email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Campo requerido" });
            _email.Validations.Add(new EmailRule());
            _name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Campo requerido" });
            _phoneNumber.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Campo requerido" });
            _lastName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Campo requerido" });
            _secondLastName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Campo requerido" });
            _vehicle.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Campo requerido" });
        }
        #endregion
    }
}
