using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Validations;
using MorrallaExpress.Validations.Rules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace MorrallaExpress.ViewModels
{
    public class ForgotPasswordViewModel : ViewModelBase
    {
        ValidatableObject<string> email = new ValidatableObject<string>();
        public ValidatableObject<string> Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public DelegateCommand RecoverPasswordCommand { get; set; }
        public ForgotPasswordViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            Title = "Recuperar contraseña";
            RecoverPasswordCommand = new DelegateCommand(RecoverPassword);
            AddValidations();
        }

        public async void RecoverPassword()
        {
           
            if (Validate())
            {
                await HttpService.ForgotPasswordService(Email.Value);
                await PopUp("Correo de recuperación enviado", "En el link podrás restablecer tu contraseña.", "Aceptar", new DelegateCommand(async () => await NavigateBack()));
            }
        }

        bool Validate()
        {
            var isValidEmail = email.Validate();
            Email.IsValid = isValidEmail;
            // MessagingCenter.Send(this, "RecoveredPassword");
            return isValidEmail;
        }

        void AddValidations()
        {
            email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* El campo es requerido" });
            email.Validations.Add(new EmailRule());
        }
    }
}
