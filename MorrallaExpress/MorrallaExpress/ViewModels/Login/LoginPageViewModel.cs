using Acr.UserDialogs;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models.DTOs;
using MorrallaExpress.Validations;
using MorrallaExpress.Validations.Rules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MorrallaExpress.ViewModels.Login
{
    public class LoginPageViewModel : ViewModelBase
    {
        #region Props
        
        ValidatableObject<string> _email = new ValidatableObject<string>();
        public ValidatableObject<string> Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        ValidatableObject<string> _password = new ValidatableObject<string>();
        public ValidatableObject<string> Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        bool _isPassword = true;
        public bool IsPassword { get => _isPassword; set => SetProperty(ref _isPassword, value); }
        #endregion

        #region Commands
        public DelegateCommand LoginCommand { get; set; }
        public DelegateCommand ToClientRegisterCommand { get; set; }
        public DelegateCommand ToFreqRegisterCommand { get; set; }
        public DelegateCommand ToForgotPasswordCommand { get; set; }
        public DelegateCommand ShowPasswordCommand { get; set; }

        #endregion
        public LoginPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            LoginCommand = new DelegateCommand(Login);
            ToClientRegisterCommand = new DelegateCommand(async () => await Navigate("RegisterClientPage"));
            ToFreqRegisterCommand = new DelegateCommand(async () => await Navigate("RegisterFranqPage"));
            ToForgotPasswordCommand = new DelegateCommand(async () => await Navigate("ForgotPassword"));
            AddValidations();
            ShowPasswordCommand = new DelegateCommand(ShowPassword);

            Title = "Iniciar sesión";
        }

        public async void Login()
        {
            // AbsoluteNavigation("/HomeMasterDetailPage/NavigationPage/MainPage");
            if (await CheckForInternet() && Validate())
            {
                var requestModel = new LoginRequestModel {
                    Email = Email.Value,
                    Password = Password.Value,
                };
                var res = false;
                using (UserDialogs.Instance.Loading("Cargando..."))
                    res = await HttpService.Login(requestModel);
                if (res)
                {
                    var rolUser = HttpService.GetCurrentUser();
                    if (rolUser.Roles.Contains("User"))
                        await AbsoluteNavigation("/HomeMasterDetailPage/NavigationPage/OrdersTabbedPage?selectedTab=PendingOrdersPage");
                    else if (rolUser.Roles.Contains("Driver"))
                        await AbsoluteNavigation("/HomeMasterDetailPage/NavigationPage/OrdersDriverTabbedPage?selectedTab=AvailableOrdersDriverPage");
                }
                else
                    await ErrorPopUp();
            }
        }

        void ShowPassword() =>
            IsPassword = !IsPassword;
        bool Validate()
        {
            var isValidEmail = _email.Validate();
            Email.IsValid = isValidEmail;

            var isValidPassword = _password.Validate();
            Password.IsValid = isValidPassword;

            return isValidEmail && isValidPassword;
        }

        void AddValidations()
        {
            _email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "El campo de correo electrónico no puede estar vacío." });
            _email.Validations.Add(new EmailRule { ValidationMessage = "El correo introducido no cumple con el formato de correo válido: prueba@correo.com" });
            _password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "El campo de contraseña no puede ser vacío." });
            _password.Validations.Add(new PasswordRule());
        }
    }
}
