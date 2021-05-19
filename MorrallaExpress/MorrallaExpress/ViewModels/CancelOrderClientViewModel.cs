using Acr.UserDialogs;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models.DTOs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;

namespace MorrallaExpress.ViewModels
{
    public class CancelOrderClientViewModel : ViewModelBase
    {
        private OrderModel _currentOrder;
        public OrderModel CurrentOrder
        {
            get { return _currentOrder; }
            set { SetProperty(ref _currentOrder, value); }
        }

        private CancellationReason _selectedReason;
        public CancellationReason SelectedReason
        {
            get { return _selectedReason; }
            set { SetProperty(ref _selectedReason, value); }
        }

        private bool _specificationReason;
        public bool SpecificationReason
        {
            get { return _specificationReason; }
            set { SetProperty(ref _specificationReason, value); }
        }

        string _reasonString = "";
        public string ReasonString
        {
            get => _reasonString;
            set => SetProperty(ref _reasonString, value);
        }

        private List<CancellationReason> _Picker = new List<CancellationReason> {
            new CancellationReason {
                IdReason = 0, Reason ="Mucho tiempo de espera"
            },
            new CancellationReason {
                IdReason = 1, Reason ="Ya conseguí morralla"
            },
            new CancellationReason {
                IdReason = 2, Reason ="Salí del comercio"
            },
            new CancellationReason {
                IdReason = 3, Reason ="Otro"
            }

        };

        public List<CancellationReason> ReasonList
        {
            get { return _Picker; }
            set { SetProperty(ref _Picker, value); }
        }
        public DelegateCommand CancelarCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand OnReasonChangeCommand { get; set; }

        public CancelOrderClientViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            CancelarCommand = new DelegateCommand(CancelOrden);
            GoBackCommand = new DelegateCommand(async () => await NavigateBack());
            OnReasonChangeCommand = new DelegateCommand(OnReasonChange);
            SelectedReason = _Picker[0];
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            CurrentOrder = (OrderModel)parameters["model"] ?? CurrentOrder;
        }

        public void OnReasonChange()
        {
            if (SelectedReason == null)
                return;
            SpecificationReason = SelectedReason.IdReason == 3;
        }

        public async void CancelOrden()
        {
            var deleteCmd = new DelegateCommand(async () =>
            {
                bool res;
                using (UserDialogs.Instance.Loading("Cargando..."))
                {
                    var pos = await Geolocation.GetLastKnownLocationAsync();
                    res = await HttpService.CancelOrderClient(CurrentOrder.DeliveryId, pos.Latitude, pos.Longitude, SelectedReason.IdReason, ReasonString);
                }
                if (res)
                    await Navigate("/HomeMasterDetailPage/NavigationPage/OrdersTabbedPage?selectedTab=PendingOrdersPage", new NavigationParameters { { "force", false } });
                else
                    await PopUp("Error", "No se puedo cancelar la orden.");
            });

            DateTime now = DateTime.UtcNow;
            TimeSpan diffMinutes = now - (CurrentOrder?.AcceptedDate ?? now);
            if (diffMinutes.Minutes > 2)
                await PopUp("Confirma tu cancelación", $"¿Estás seguro que deseas cancelar tu orden? \n Se le cobrará una comisión de 50 pesos.", "Aceptar", deleteCmd, "Cancelar", null);
            else
                await PopUp("Confirma tu cancelación", $"¿Estás seguro que deseas cancelar tu orden?", "Aceptar", deleteCmd, "Cancelar", null);

        }
    }
}
