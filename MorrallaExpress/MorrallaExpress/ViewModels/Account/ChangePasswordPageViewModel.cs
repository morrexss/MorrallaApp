using Acr.UserDialogs;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Validations;
using MorrallaExpress.Validations.Rules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MorrallaExpress.ViewModels.Account
{
    public class ChangePasswordPageViewModel : ViewModelBase
    {
        public DelegateCommand SaveNewPassword { get; set; }
        public DelegateCommand ShowPasswordCommand { get; set; }

        bool _isPassword = true;
        public bool IsPassword { get => _isPassword; set => SetProperty(ref _isPassword, value); }

        ValidatableObject<string> _oldPw = new ValidatableObject<string>();
        public ValidatableObject<string> OldPassword
        {
            get => _oldPw;
            set => SetProperty(ref _oldPw, value);
        }

        ValidatableObject<string> _password = new ValidatableObject<string>();
        public ValidatableObject<string> Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        
        ValidatableObject<string> _confirmPassword = new ValidatableObject<string>();
        public ValidatableObject<string> ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }
        public ChangePasswordPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            Title = "Cambiar contraseña";
            SaveNewPassword = new DelegateCommand(Save);
            ShowPasswordCommand = new DelegateCommand(ShowPassword);

            AddValidations();
        }

        public async void Save()
        {
            if (Validate1())
            {
                var usr = HttpService.GetCurrentUser();
                var res = false;
                using (UserDialogs.Instance.Loading("Cargando..."))
                    res = await HttpService.ResetPassword(usr.Email, OldPassword.Value, Password.Value);
                if (res)
                    await PopUp("¡Listo!", "Tu contraseña se ha cambiado correctamente.", "Aceptar", new DelegateCommand(async () => await NavigateBack()));
                else
                    await PopUp("Algo salió mal", "Ocurrió un error al procesar tu solicitud.", "Aceptar");
            }
            else
                await PopUp("Contraseñas no coinciden", "Por favor inténtalo de nuevo.", "Aceptar");
        }
        void ShowPassword() =>
            IsPassword = !IsPassword;

        bool Validate1()
        {
            //var isValidOldPasswd = _oldPw.Validate();
            //OldPassword.IsValid = isValidOldPasswd;

            var isValidPassword = _password.Validate();
            Password.IsValid = isValidPassword;

            var isValidRConfirmPassword = _confirmPassword.Validate() && _confirmPassword.Value.Equals(_password.Value);
            ConfirmPassword.IsValid = isValidRConfirmPassword;

            return /*isValidOldPasswd && */isValidPassword && isValidRConfirmPassword;
        }

        void AddValidations()
        {

            _oldPw.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* El campo no debe ser vacío." });
            _password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* El campo de contraseña no debe ser vacío." });
            _password.Validations.Add(new PasswordRule { ValidationMessage = "* La contraseña al menos debe tener 8 caracteres." });
            _confirmPassword.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* El campo de confirmar contraseña no debe ser vacío." });
            _confirmPassword.Comparison.Add(new ComparePasswordsRule { ValidationMessage = "* Las contraseñas no coinciden", ValueToCompare = Password });
        }
    }
}
