using Acr.UserDialogs;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using MorrallaExpress.Models.DTOs;
using MorrallaExpress.Validations;
using MorrallaExpress.Validations.Rules;
using Openpay.Xamarin;
using Openpay.Xamarin.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MorrallaExpress.ViewModels
{
    public class PaymentsDetailPageViewModel : ViewModelBase
    {
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }

        #region Props
        PaymentMethodModel _selected;
        bool _isEdit;
        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }
        bool _entriesEnabled = true;
        public bool EntriesEnabled
        {
            get => _entriesEnabled;
            set => SetProperty(ref _entriesEnabled, value);
        }
        string _deleteIconSource;
        public string DeleteIconSource
        {
            get => _deleteIconSource;
            set => SetProperty(ref _deleteIconSource, value);
        }
        ValidatableObject<string> _holderName = new ValidatableObject<string>();
        public ValidatableObject<string> HolderName
        {
            get => _holderName;
            set => SetProperty(ref _holderName, value);
        }
        ValidatableObject<string> _cardNumber = new ValidatableObject<string>();
        public ValidatableObject<string> CardNumber
        {
            get => _cardNumber;
            set => SetProperty(ref _cardNumber, value);
        }
        ValidatableObject<string> _cvv = new ValidatableObject<string>();
        public ValidatableObject<string> Cvv
        {
            get => _cvv;
            set => SetProperty(ref _cvv, value);
        }

        ValidatableObject<string> _selectedMonth = new ValidatableObject<string>();
        public ValidatableObject<string> SelectedMonth
        {
            get => _selectedMonth;
            set => SetProperty(ref _selectedMonth, value);
        }
        ValidatableObject<string> _selectedYear = new ValidatableObject<string>();
        public ValidatableObject<string> SelectedYear
        {
            get => _selectedYear;
            set => SetProperty(ref _selectedYear, value);
        }
        List<string> _monthList = new List<string>();
        public List<string> MonthList
        {
            get => _monthList;
            set => SetProperty(ref _monthList, value);
        }
        List<string> _yearList = new List<string>();
        public List<string> YearList
        {
            get => _yearList;
            set => SetProperty(ref _yearList, value);
        }
        String _DeviceId;
        public string DeviceId { get => _DeviceId; set => SetProperty(ref _DeviceId, value); }
        #endregion

        public PaymentsDetailPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            Title = "Nuevo método de pago";
            SaveCommand = new DelegateCommand(Save);
            DeleteCommand = new DelegateCommand(Delete);
            #region NoVer
            MonthList.Add("01");
            MonthList.Add("02");
            MonthList.Add("03");
            MonthList.Add("04");
            MonthList.Add("05");
            MonthList.Add("06");
            MonthList.Add("07");
            MonthList.Add("08");
            MonthList.Add("09");
            MonthList.Add("10");
            MonthList.Add("11");
            MonthList.Add("12");
            YearList.Add("19");
            YearList.Add("20");
            YearList.Add("21");
            YearList.Add("22");
            YearList.Add("23");
            YearList.Add("24");
            YearList.Add("25");
            SelectedMonth.Value = MonthList[0];
            SelectedYear.Value = YearList[0];
            #endregion
            AddValidations();
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters["model"] is PaymentMethodModel model)
            {
                IsEdit = true;
                Title = $"Detalle";
                _selected = model;
                HolderName.Value = model.HolderName;
                CardNumber.Value = model.MaskedCard;
                DeleteIconSource = "DeleteIcn.png";
                EntriesEnabled = false;
                if (!string.IsNullOrEmpty(model.Expiration)) {
                    SelectedMonth.Value = model.Expiration.Split('/')[0];
                    SelectedYear.Value = model.Expiration.Split('/')[1];
                }
            }
            else
                Title = $"Nuevo método de pago";
        }
        async void Save()
        {
            if (Validate())
            {
                string deviceSessionId = string.Empty;
                Token cardToken = null;
                if (CrossOpenpay.IsSupported)
                {
                    deviceSessionId = await CrossOpenpay.Current.CreateDeviceSessionId();
                    cardToken = await CrossOpenpay.Current.CreateTokenFromCard(new Card {
                        HolderName = HolderName.Value,
                        Number = CardNumber.Value,
                        Cvv2 = int.Parse(Cvv.Value),
                        ExpirationMonth = SelectedMonth.Value,
                        ExpirationYear = SelectedYear.Value,
                        AllowCharges = true
                    });
                }
                if (string.IsNullOrEmpty(deviceSessionId) || cardToken is null)
                {
                    await PopUp("Error", "No se pudo conectar con OpenPay.");
                    return;
                }
                using (UserDialogs.Instance.Loading("Cargando..."))
                {
                    var model = GenerateModel(deviceSessionId);
                    model.TokenId = cardToken.Id;
                    
                    var res = await HttpService.CreatePaymentMethod(model);
                    if (res)
                        await NavigateBack();
                    else
                        await PopUp("Error", "No se pudo guardar tu método de pago.");
                }
            }
        }

        async void Delete()
        {
            var deleteCmd = new DelegateCommand(async () => {
                using (UserDialogs.Instance.Loading("Cargando..."))
                    await HttpService.DeletePaymentMethod(_selected.PaymentMethodId);
                await NavigateBack();
            });

            await PopUp("Advertencia", $"Estás seguro que deseas eliminar {_selected.MaskedCard}?",
                "Aceptar", deleteCmd, "Cancelar", null);
        }


        PaymentMethodModel GenerateModel(string devId) => new PaymentMethodModel
        {
            PaymentMethodId = _selected != null ? _selected.PaymentMethodId : 0,
            CardNumber = CardNumber.Value,
            DeviceId = devId,
            Expiration = $"{SelectedMonth.Value}/{SelectedYear.Value}",
            Cvv = Cvv.Value,
            HolderName = HolderName.Value,
            UserId = HttpService.GetCurrentUser().UserId,
        };

        #region FieldValidations
        bool Validate()
        {
            var isValidName = _holderName.Validate();
            HolderName.IsValid = isValidName;

            var isValidCardNumber = _cardNumber.Validate();
            CardNumber.IsValid = isValidCardNumber;

            var isValidCvv = _cvv.Validate();
            Cvv.IsValid = isValidCvv;

            var isValidMonth = _selectedMonth.Validate();
            SelectedMonth.IsValid = isValidMonth;

            var isValidYear = _selectedYear.Validate();
            SelectedYear.IsValid = isValidYear;

            return isValidName && isValidCardNumber && isValidCvv &&
                isValidMonth && isValidYear;
        }

        void AddValidations()
        {
            _holderName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* Campo requerido" });
            _cardNumber.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* Campo requerido" });
            _cvv.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* Campo requerido" });
            _selectedMonth.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* Campo requerido" });
            _selectedYear.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* Campo requerido" });
        }
        #endregion
    }
}
