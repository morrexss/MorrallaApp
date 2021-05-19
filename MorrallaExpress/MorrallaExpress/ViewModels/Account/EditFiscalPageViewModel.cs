using Acr.UserDialogs;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using MorrallaExpress.Models.DTOs;
using MorrallaExpress.Validations;
using MorrallaExpress.Validations.Rules;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MorrallaExpress.ViewModels.Account
{
    public class EditFiscalPageViewModel : ViewModelBase
    {
        #region Commands
        public DelegateCommand UpdateCommand { get; set; }
        #endregion

        #region Props
        readonly static string _requiredValidationMessage = "* Campo requerido";

        UsoCFDIModel[] _usoCFDIsList;
        public UsoCFDIModel[] UsoCFDIsList
        {
            get => _usoCFDIsList;
            set => SetProperty(ref _usoCFDIsList, value);
        }

        public StateModel[] _states = new StateModel[] { };
        public StateModel[] States
        {
            get => _states;
            set => SetProperty(ref _states, value);
        }

        ValidatableObject<string> _companyName = new ValidatableObject<string>();
        public ValidatableObject<string> CompanyName
        {
            get => _companyName;
            set => SetProperty(ref _companyName, value);
        }
        ValidatableObject<string> _fiscalName = new ValidatableObject<string>();
        public ValidatableObject<string> FiscalName
        {
            get => _fiscalName;
            set => SetProperty(ref _fiscalName, value);
        }
        ValidatableObject<string> _rfc = new ValidatableObject<string>();
        public ValidatableObject<string> Rfc
        {
            get => _rfc;
            set => SetProperty(ref _rfc, value);
        }
        ValidatableObject<UsoCFDIModel> _usoCFDI = new ValidatableObject<UsoCFDIModel>();
        public ValidatableObject<UsoCFDIModel> UsoCFDI
        {
            get => _usoCFDI;
            set => SetProperty(ref _usoCFDI, value);
        }
        ValidatableObject<string> _fiscalAddress = new ValidatableObject<string>();
        public ValidatableObject<string> FiscalAddress
        {
            get => _fiscalAddress;
            set => SetProperty(ref _fiscalAddress, value);
        }
        ValidatableObject<CityModel> _fiscalCity = new ValidatableObject<CityModel>();
        public ValidatableObject<CityModel> FiscalCity
        {
            get => _fiscalCity;
            set => SetProperty(ref _fiscalCity, value);
        }
        ValidatableObject<StateModel> _fiscalState = new ValidatableObject<StateModel>();
        public ValidatableObject<StateModel> FiscalState
        {
            get => _fiscalState;
            set => SetProperty(ref _fiscalState, value);
        }
        ValidatableObject<string> _fiscalDelegation = new ValidatableObject<string>();
        public ValidatableObject<string> FiscalDelegation
        {
            get => _fiscalDelegation;
            set => SetProperty(ref _fiscalDelegation, value);
        }
        ValidatableObject<string> _fiscalPostalCode = new ValidatableObject<string>();
        public ValidatableObject<string> FiscalPostalCode
        {
            get => _fiscalPostalCode;
            set => SetProperty(ref _fiscalPostalCode, value);
        }
        #endregion

        public EditFiscalPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            Title = "Editar información fiscal";
            UpdateCommand = new DelegateCommand(UpdateInformation);
            AddValidations();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            using (UserDialogs.Instance.Loading("Cargando..."))
            {
                var user = HttpService.GetCurrentUser();

                States = (await HttpService.GetAvailableStates()).ToArray();
                UsoCFDIsList = (await HttpService.GetCFDIs()).ToArray();

                CompanyName.Value = user.CompanyName;
                FiscalName.Value = user.FiscalName;
                Rfc.Value = user.Rfc;
                UsoCFDI.Value = UsoCFDIsList.FirstOrDefault(s => s.Id == user.UsoId);
                FiscalAddress.Value = user.FiscalAddress;
                FiscalState.Value = States.FirstOrDefault(s => s.Name == user.FiscalState);
                FiscalCity.Value = FiscalState.Value.Cities.FirstOrDefault(c => c.Name == user.FiscalCity || c.CityId == user.CityId); ;
                FiscalPostalCode.Value = user.FiscalPostalCode;
            }
        }

        public async void UpdateInformation()
        {
            if (!InfoValidation())
                return;

            var currentUser = HttpService.GetCurrentUser();

            currentUser.CompanyName = CompanyName.Value;
            currentUser.FiscalName = FiscalName.Value;
            currentUser.Rfc = Rfc.Value;
            currentUser.UsoId = UsoCFDI.Value.Id;
            currentUser.FiscalAddress = FiscalAddress.Value;
            currentUser.FiscalState = FiscalState.Value.Name;
            currentUser.FiscalCity = FiscalCity.Value.Name;
            currentUser.FiscalPostalCode = FiscalPostalCode.Value;

            var res = false;
            using (UserDialogs.Instance.Loading("Cargando..."))
                res = await HttpService.UpdateUserData(currentUser);
            if (res)
                await NavigationService.GoBackAsync();
            else
                await ErrorPopUp();
        }

        #region -  VALIDATIONS  -
        void AddValidations()
        {
            _companyName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _fiscalName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _rfc.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _usoCFDI.Validations.Add(new IsNotNullOrEmptyRule<UsoCFDIModel> { ValidationMessage = _requiredValidationMessage });
            _fiscalAddress.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _fiscalCity.Validations.Add(new IsNotNullOrEmptyRule<CityModel> { ValidationMessage = _requiredValidationMessage });
            _fiscalState.Validations.Add(new IsNotNullOrEmptyRule<StateModel> { ValidationMessage = _requiredValidationMessage });
            _fiscalDelegation.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _fiscalPostalCode.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
        }

        bool InfoValidation()
        {
            var isValidCompanyName = _companyName.Validate();
            CompanyName.IsValid = isValidCompanyName;

            var isValidFiscalName = _fiscalName.Validate();
            _fiscalName.IsValid = isValidFiscalName;

            var isValidRfc = _rfc.Validate();
            Rfc.IsValid = isValidRfc;

            var isValidUsoCfdi = _usoCFDI.Validate();
            UsoCFDI.IsValid = isValidUsoCfdi;

            var isValidFiscalAddress = _fiscalAddress.Validate();
            FiscalAddress.IsValid = isValidFiscalAddress;

            var isValidFiscalCity = _fiscalCity.Validate();
            FiscalCity.IsValid = isValidFiscalCity;

            var isValidFiscalState = _fiscalState.Validate();
            FiscalState.IsValid = isValidFiscalState;

            var isValidFiscalPostalCode = _fiscalPostalCode.Validate();
            _fiscalPostalCode.IsValid = isValidFiscalPostalCode;

            return isValidCompanyName && isValidFiscalName && isValidRfc && isValidUsoCfdi &&
                isValidFiscalAddress && isValidFiscalCity && isValidFiscalState &
                isValidFiscalPostalCode;
        }
        #endregion

    }

}
