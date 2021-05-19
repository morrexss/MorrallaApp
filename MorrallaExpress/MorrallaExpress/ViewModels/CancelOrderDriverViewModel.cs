using Acr.UserDialogs;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models.DTOs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace MorrallaExpress.ViewModels
{
    public class CancelOrderDriverViewModel : ViewModelBase
    {
        #region Props
        private List<CancellationReason> _Picker = new List<CancellationReason> {
            new CancellationReason {
                IdReason = 0, Reason ="Zona o local inseguro"
            },
            new CancellationReason {
                IdReason = 1, Reason ="Distancia"
            },
            new CancellationReason {
                IdReason = 2, Reason ="Clima"
            },
             new CancellationReason {
                IdReason = 3, Reason ="Trafico"
            },
             new CancellationReason {
                IdReason = 4, Reason ="Otro"
            }
        };
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

        private bool _specificationReason = false;
        public bool SpecificationReason
        {
            get { return _specificationReason; }
            set { SetProperty(ref _specificationReason, value); }
        }
        #endregion
        #region Commands
        public DelegateCommand CancelarCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand OnReasonChangeCommand { get; set; }
        #endregion
        string _reasonString = "";
        public string ReasonString
        {
            get => _reasonString;
            set => SetProperty(ref _reasonString, value);
        }

        public List<CancellationReason> ReasonList
        {
            get { return _Picker; }
            set { SetProperty(ref _Picker, value); }
        }
        public CancelOrderDriverViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            CancelarCommand = new DelegateCommand(CancelOrden);
            GoBackCommand = new DelegateCommand(async () => await NavigateBack());
            OnReasonChangeCommand = new DelegateCommand(OnReasonChange);


        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            CurrentOrder = (OrderModel)parameters["model"] ?? CurrentOrder;
        }
        public void OnReasonChange()
        {
            if (SelectedReason.IdReason == 4)
                SpecificationReason = true;
            else
                SpecificationReason = false;
        }
        public async void CancelOrden()
        {
            var deleteCmd = new DelegateCommand(async () =>
            {
                bool res = false;
                using (UserDialogs.Instance.Loading("Cargando..."))
                {
                    var pos = await Geolocation.GetLastKnownLocationAsync();
                    res = await HttpService.CancelOrder(CurrentOrder.DeliveryId, pos.Latitude, pos.Longitude, SelectedReason.IdReason, ReasonString);
                }

                if (res)
                    await Navigate("/HomeMasterDetailPage/NavigationPage/OrdersDriverTabbedPage?selectedTab=AvailableOrdersDriverPage", new NavigationParameters { { "force", false } });
                else
                    await PopUp("Error", "No se puedo cancelar la orden.");
            });

            await PopUp("Advertencia", "¿Estás seguro que deseas cancelar la orden?", "Aceptar", deleteCmd, "Cancelar", null);
        }
    }
}
