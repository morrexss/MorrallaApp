using Acr.UserDialogs;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using MorrallaExpress.Models.DTOs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MorrallaExpress.ViewModels
{
    public class NewOrderPageViewModel : ViewModelBase
    {
        #region Lists
        private LocationModel[] _locations;
        public LocationModel[] Locations
        {
            get { return _locations; }
            set { SetProperty(ref _locations, value); }
        }
        private PaymentMethodModel[] _paymentMethods;
        public PaymentMethodModel[] PaymentMethods
        {
            get { return _paymentMethods; }
            set { SetProperty(ref _paymentMethods, value); }
        }
        private DenominationModel[] _denominations = new DenominationModel[] { };
        public DenominationModel[] Denominations
        {
            get { return _denominations; }
            set { SetProperty(ref _denominations, value); }
        }
        private SettingsModel[] _settings = new SettingsModel[] { };
        public SettingsModel[] Settings
        {
            get { return _settings; }
            set { SetProperty(ref _settings, value); }
        }
        #region Selected
        LocationModel _selectedLocation;
        public LocationModel SelectedLocation
        {
            get { return _selectedLocation; }
            set { SetProperty(ref _selectedLocation, value); }
        }
        PaymentMethodModel _selectedPaymentMethod;
        public PaymentMethodModel SelectedPaymentMethod
        {
            get { return _selectedPaymentMethod; }
            set { SetProperty(ref _selectedPaymentMethod, value); }
        }
        DenominationItem _selectedDenominationOne;
        public DenominationItem SelectedDenominationOne
        {
            get { return _selectedDenominationOne; }
            set { SetProperty(ref _selectedDenominationOne, value); }
        }
        DenominationItem _selectedDenominationTwo;
        public DenominationItem SelectedDenominationTwo
        {
            get { return _selectedDenominationTwo; }
            set { SetProperty(ref _selectedDenominationTwo, value); }
        }
        DenominationItem _selectedDenominationFive;
        public DenominationItem SelectedDenominationFive
        {
            get { return _selectedDenominationFive; }
            set { SetProperty(ref _selectedDenominationFive, value); }
        }
        DenominationItem _selectedDenominationTen;
        public DenominationItem SelectedDenominationTen
        {
            get { return _selectedDenominationTen; }
            set { SetProperty(ref _selectedDenominationTen, value); }
        }
        DenominationItem _selectedDenominationTwenty;
        public DenominationItem SelectedDenominationTwenty
        {
            get { return _selectedDenominationTwenty; }
            set { SetProperty(ref _selectedDenominationTwenty, value); }
        }
        DenominationItem _selectedDenominationFifty;
        public DenominationItem SelectedDenominationFifty
        {
            get { return _selectedDenominationFifty; }
            set { SetProperty(ref _selectedDenominationFifty, value); }
        }
        #endregion
        #endregion

        private bool _isEditing;
        public bool IsEditing
        {
            get { return _isEditing; }
            set { SetProperty(ref _isEditing, value); }
        }
        private OrderModel _currentOrder;
        public OrderModel CurrentOrder
        {
            get { return _currentOrder; }
            set { SetProperty(ref _currentOrder, value); }
        }
        private decimal _morrexssTotal;
        public decimal MorrexssTotal
        {
            get { return _morrexssTotal; }
            set { SetProperty(ref _morrexssTotal, value); }
        }
        private decimal _paymentTotal;
        public decimal PaymentTotal
        {
            get { return _paymentTotal; }
            set { SetProperty(ref _paymentTotal, value); }
        }
        private bool _PaymentTotalModal = false;
        public bool PaymentTotalModal
        {
            get { return _PaymentTotalModal; }
            set { SetProperty(ref _PaymentTotalModal, value); }
        }
        private bool _MorrexssTotalModal = false;
        public bool MorrexssTotalModal
        {
            get { return _MorrexssTotalModal; }
            set { SetProperty(ref _MorrexssTotalModal, value); }
        }

        public DelegateCommand<DenominationItem> PickerChangedCommand { get; set; }
        public DelegateCommand CreateOrderCommand { get; set; }
        public DelegateCommand LocationsPickerFocusCommand { get; set; }
        public DelegateCommand PaymentsPickerFocusCommand { get; set; }
        public DelegateCommand ToDenominationsCommand { get; set; }
        public DelegateCommand PaymentTotalModalCommand { get; set; }
        public DelegateCommand MorrexxsTotalModalCommand { get; set; }

        public NewOrderPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            PickerChangedCommand = new DelegateCommand<DenominationItem>(PickerChanged);
            PaymentTotalModalCommand = new DelegateCommand(PaymetModal);
            MorrexxsTotalModalCommand = new DelegateCommand(MorrexssModal);

            LocationsPickerFocusCommand = new DelegateCommand(async () => await LocationsPickerFocus());
            PaymentsPickerFocusCommand = new DelegateCommand(async () => await PaymentsPickerFocus());

            CreateOrderCommand = new DelegateCommand(TryCreateOrder);
            ToDenominationsCommand = new DelegateCommand(async () => await ToDenominations());

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            Locations = HttpService.GetCurrentLocations();
            PaymentMethods = HttpService.GetCurrentPaymentMethods();

            if (!Denominations.Any() || !Settings.Any())
            {
                Denominations = HttpService.GetCurrentDenominations();
                Settings = HttpService.GetCurrentSettings();
            }

            decimal MontoMaximo = Convert.ToDecimal(Settings[2].Value);

            foreach (var den in Denominations)
            {
                int packing = den.Packaging;
                int qty = 1;
                den.Details = new DenominationItem[] {
                    new DenominationItem
                    {
                        DenominationId = den.DenominationId,
                        Value = 0,
                        Quantity = 0
                    }
                };
                if (MorrexssTotal == 0)
                {
                    PickerChanged(den.Details[0]);
                }
                for (int i = packing; i <= MontoMaximo; i += packing)
                {
                    var detail = new DenominationItem
                    {
                        DenominationId = den.DenominationId,
                        Value = i,
                        Quantity = qty++
                    };
                    var list = den.Details.ToList();
                    list.Add(detail);
                    den.Details = list.ToArray();
                }
            }

            //Denominations = HttpService.GetCurrentDenominations();
            // Settings = HttpService.GetCurrentSettings();
            // acaba feo


            if (parameters["model"] is OrderModel model)
            {
                IsEditing = true;
                CurrentOrder = model;
            }
        }
        public void PaymetModal()
        {
            PaymentTotalModal = !PaymentTotalModal;
        }
        public void MorrexssModal()
        {
            MorrexssTotalModal = !MorrexssTotalModal;
        }
        async Task LocationsPickerFocus()
        {
            if (!Locations.Any())
            {
                var acceptCmd = new DelegateCommand(async () => await Navigate("LocationDetailPage"));
                await PopUp("Advertencia", "No tienes sucursales, es requerido mínimo una para continuar.", "Aceptar", acceptCmd);
            }
        }

        async Task PaymentsPickerFocus()
        {
            if (!PaymentMethods.Any())
            {
                var acceptCmd = new DelegateCommand(async () => await Navigate("PaymentsDetailPage"));
                await PopUp("Advertencia", "No tienes métodos de pago, es requerido mínimo uno para continuar.", "Aceptar", acceptCmd);
            }
        }

        async void TryCreateOrder()
        {
            if (SelectedLocation == null)
            {
                await UserDialogs.Instance.AlertAsync("Seleccione una sucursal", "Advertencia", "Aceptar");
                return;
            }
            if (SelectedPaymentMethod == null)
            {
                await UserDialogs.Instance.AlertAsync("Seleccione su método de pago", "Advertencia", "Aceptar");
                return;
            }
            if (MorrexssTotal > 20000)
            {
                await UserDialogs.Instance.AlertAsync("La cantidad máxima es de $20,000.00 MXN", "Advertencia", "Aceptar");
                return;
            }
            if (MorrexssTotal == 0)
            {
                await UserDialogs.Instance.AlertAsync("La cantidad no puede ser $00.00 MXN", "Advertencia", "Aceptar");
                return;
            }

            var acceptCmd = new DelegateCommand(async () =>
            {
                bool res = false;
                using (UserDialogs.Instance.Loading("Cargando..."))
                    res = await HttpService.CreateOrEditOrder(GenerateModel());

                if (!res)
                    await UserDialogs.Instance.AlertAsync("No se pudo crear la orden", "Advertencia", "Aceptar");
                else
                    await NavigateBack();
            });

            await PopUp("Confirma tu orden", $"¿Seguro que deseas tu morralla?", "Aceptar", acceptCmd, "Cancelar", null);
        }

        void PickerChanged(DenominationItem selected)
        {
            switch (selected.DenominationId)
            {
                case 1:
                    SelectedDenominationOne = selected;
                    break;
                case 2:
                    SelectedDenominationTwo = selected;
                    break;
                case 3:
                    SelectedDenominationFive = selected;
                    break;
                case 4:
                    SelectedDenominationTen = selected;
                    break;
                case 5:
                    SelectedDenominationTwenty = selected;
                    break;
                case 6:
                    SelectedDenominationFifty = selected;
                    break;
            }
            decimal all = 0;
            if (SelectedDenominationOne != null)
                all += SelectedDenominationOne.Value;
            if (SelectedDenominationTwo != null)
                all += SelectedDenominationTwo.Value;
            if (SelectedDenominationFive != null)
                all += SelectedDenominationFive.Value;
            if (SelectedDenominationTen != null)
                all += SelectedDenominationTen.Value;
            if (SelectedDenominationTwenty != null)
                all += SelectedDenominationTwenty.Value;
            if (SelectedDenominationFifty != null)
                all += SelectedDenominationFifty.Value;

            MorrexssTotal = all;
            var pricing = HttpService.GetCurrentPricings();
            foreach (var item in pricing)
            {
                if (MorrexssTotal >= item.MinRange && MorrexssTotal <= item.MaxRange)
                    PaymentTotal = (decimal)item.MorrexComision;
            }
        }

        async Task ToDenominations()
        {
            await NavigationService.NavigateAsync("DenominationsPage", new NavigationParameters { { "Denominations", Denominations.ToList() } });
        }

        OrderModelDTO GenerateModel() => new OrderModelDTO
        {
            ApplicationUserId = HttpService.GetCurrentUser().UserId,
            LocationId = SelectedLocation.LocationId,
            CardId = SelectedPaymentMethod.CardId,
            Details = GenerateOrderDetails()
        };

        OrderDetailModelDTO[] GenerateOrderDetails()
        {
            List<OrderDetailModelDTO> details = new List<OrderDetailModelDTO>();
            if (SelectedDenominationOne != null)
                details.Add(new OrderDetailModelDTO
                {
                    DenominationId = SelectedDenominationOne.DenominationId,
                    Quantity =  (int)SelectedDenominationOne.Value
                });
            if (SelectedDenominationTwo != null)
                details.Add(new OrderDetailModelDTO
                {
                    DenominationId = SelectedDenominationTwo.DenominationId,
                    Quantity = (int)SelectedDenominationTwo.Value
                });
            if (SelectedDenominationFive != null)
                details.Add(new OrderDetailModelDTO
                {
                    DenominationId = SelectedDenominationFive.DenominationId,
                    Quantity =  (int)SelectedDenominationFive.Value
                });
            if (SelectedDenominationTen != null)
                details.Add(new OrderDetailModelDTO
                {
                    DenominationId = SelectedDenominationTen.DenominationId,
                    Quantity = (int)SelectedDenominationTen.Value
                });
            if (SelectedDenominationTwenty != null)
                details.Add(new OrderDetailModelDTO
                {
                    DenominationId = SelectedDenominationTwenty.DenominationId,
                    Quantity =  (int)SelectedDenominationTwenty.Value
                });
            return details.ToArray();
        }
    }
}
