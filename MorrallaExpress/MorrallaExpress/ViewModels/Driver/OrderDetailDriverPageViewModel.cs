using Acr.UserDialogs;
using MorrallaExpress.Helpers;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models.DTOs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Map = Xamarin.Forms.Maps.Map;

namespace MorrallaExpress.ViewModels
{
    public class OrderDetailDriverPageViewModel : ViewModelBase
    {
        #region Props

        private decimal _totalExchange;
        public decimal TotalExchange
        {
            get { return _totalExchange; }
            set { SetProperty(ref _totalExchange, value); }
        }

        private string _acceptedDate;
        public string AcceptedDate
        {
            get { return _acceptedDate; }
            set { SetProperty(ref _acceptedDate, value); }
        }

        private bool _isNew;
        public bool IsNew
        {
            get { return _isNew; }
            set { SetProperty(ref _isNew, value); }
        }

        private bool _isCancelable;
        public bool IsCancelable
        {
            get { return _isCancelable; }
            set { SetProperty(ref _isCancelable, value); }
        }

        private bool _isDeliverable;
        public bool IsDeliverable
        {
            get { return _isDeliverable; }
            set { SetProperty(ref _isDeliverable, value); }
        }

        private int _denominationsHeight;
        public int DenominationsHeight
        {
            get { return _denominationsHeight; }
            set { SetProperty(ref _denominationsHeight, value); }
        }

        private OrderModel _currentOrder;
        public OrderModel CurrentOrder
        {
            get { return _currentOrder; }
            set { SetProperty(ref _currentOrder, value); }
        }

        private bool _showAcceptedTime;
        public bool ShowAcceptedTime
        {
            get { return _showAcceptedTime; }
            set { SetProperty(ref _showAcceptedTime, value); }
        }

        private bool _showDeliveredTime;
        public bool ShowDeliveredTime
        {
            get { return _showDeliveredTime; }
            set { SetProperty(ref _showDeliveredTime, value); }
        }
        private string _status;
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }
        private string _textStatus;
        public string TextStatus
        {
            get { return _textStatus; }
            set { SetProperty(ref _textStatus, value); }
        }

        Position _position;
        public Position Position { get => _position; set => SetProperty(ref _position, value); }


        public Map _MyMap;
        public Map MyMap { get => _MyMap; set => SetProperty(ref _MyMap, value); }

        #endregion

        #region Commands
        public DelegateCommand AcceptOrderCommand { get; set; }
        public DelegateCommand CancelOrderCommand { get; set; }
        public DelegateCommand DeliverOrderCommand { get; set; }
        public DelegateCommand OpenMapCommand { get; set; }
        public Command BackkCommand { get; }

        #endregion

        public OrderDetailDriverPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            AcceptOrderCommand = new DelegateCommand(AcceptOrder);
            CancelOrderCommand = new DelegateCommand(CancelOrder);
            DeliverOrderCommand = new DelegateCommand(DeliverOrder);
            OpenMapCommand = new DelegateCommand(OpenMap);
            BackkCommand = new Command(async () => { await PopToRoot(); });
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters["model"] is OrderModel model)
            {
                IsNew = model.Status == Constants.NewOrderStatus;
                IsCancelable = model.Status == Constants.AcceptedOrderStatus;
                IsDeliverable = model.Status == Constants.AcceptedOrderStatus;

                model.Details = model.Details.Where(w => w.Quantity > 0).ToArray();
                DenominationsHeight = model.Details.Count() * 45;

                CurrentOrder = model;
                ShowAcceptedTime = model.Status == Constants.DeliveredOrderStatus || model.Status == Constants.CanceledByUserdOrderStatus || model.Status == Constants.CanceledByDriverOrderStatus;
                ShowDeliveredTime = model.Status == Constants.DeliveredOrderStatus;
                AcceptedDate = model?.AcceptedDate?.ToString("0:hh:mm");
                Status = CurrentOrder.Status;
                TotalExchange = CurrentOrder.TotalExchange;
                SetMap();

                if (Status == "accepted")
                {
                    TextStatus = "Aceptado";
                }
                else if (Status == "new")
                {
                    TextStatus = "Nuevo";
                }
                else if (Status == "delivered")
                {
                    TextStatus = "Entregado";
                }
                else if (Status == "cancelledbyuser")
                {
                    TextStatus = "Cancelado por usuario";
                }
                else if (Status == "cancelledbydriver")
                {
                    TextStatus = "Cancelado por driver";
                }
            }
        }

        void SetMap()
        {
            var lat = CurrentOrder?.Location?.Latitude ?? 0;
            var lon = CurrentOrder?.Location?.Longitude ?? 0;
            Position positionMap = new Position(lat, lon);

            if (positionMap.Latitude != 0 && positionMap.Longitude != 0)
            {
                Pin pin = new Pin
                {
                    Label = CurrentOrder?.Location?.Name ?? "Sucursal",
                    Type = PinType.Place,
                    Position = positionMap
                };
                MyMap.Pins.Add(pin);
            }

            MyMap.MoveToRegion(new MapSpan(positionMap, 0.01, 0.01));
        }

        public void OpenMap()
        {
            var lat = CurrentOrder?.Location?.Latitude ?? 0;
            var lon = CurrentOrder?.Location?.Longitude ?? 0;
            string url = $"https://www.google.com/maps/dir/?api=1&destination={lat},{lon}&travelmode=driving";
            Launcher.OpenAsync(url);
        }

        async void CancelOrder()
        {
            await NavigationService.NavigateAsync("CancelOrderDriver", new NavigationParameters { { "model", CurrentOrder } });
            
        }

        async void DeliverOrder()
        {
            var deliverCmd = new DelegateCommand(async () =>
            {
                bool res = false;
                using (UserDialogs.Instance.Loading("Cargando..."))
                {
                    var pos = await Geolocation.GetLastKnownLocationAsync();
                    res = await HttpService.DeliverOrder(CurrentOrder.DeliveryId, pos.Latitude, pos.Longitude);
                }

                if (res)
                    await Navigate("/HomeMasterDetailPage/NavigationPage/OrdersDriverTabbedPage?selectedTab=AvailableOrdersDriverPage", new NavigationParameters { { "force", false } });
                else
                    await PopUp("Error", "No se puedo entregar la orden.");
            });

            await PopUp("Confirma tu entrega", $"¿Estás seguro que deseas confirmar tu entrega?", "Aceptar", deliverCmd, "Cancelar", null);
        }

        async void AcceptOrder()
        {
            var acceptCmd = new DelegateCommand(async () =>
            {
                bool res = false;
                using (UserDialogs.Instance.Loading("Cargando..."))
                {
                    var pos = await Geolocation.GetLastKnownLocationAsync();
                    res = await HttpService.AcceptOrder(CurrentOrder.DeliveryId, pos.Latitude, pos.Longitude);
                    
                }

                if (res)
                {
                    await Navigate("/HomeMasterDetailPage/NavigationPage/OrdersDriverTabbedPage?selectedTab=ActiveOrdersDriverPage", new NavigationParameters { { "force", false } });

                }
                else
                    await PopUp("Error", "No se te pudo asignar la orden.");
            });

            await PopUp("Confirma tu orden", $"¿Deseas aceptar la orden?", "Aceptar", acceptCmd, "Cancelar", null);
        }
    }
}
