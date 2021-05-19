using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models.DTOs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using MorrallaExpress.Controls;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Xamarin.Essentials;
using Xamarin.Forms.OpenWhatsApp;
using MorrallaExpress.Helpers;

namespace MorrallaExpress.ViewModels.Orders
{
    public class OrderDetailPageViewModel : ViewModelBase
    {
        #region Props
        private bool _isEditing;
        public bool IsEditing
        {
            get { return _isEditing; }
            set { SetProperty(ref _isEditing, value); }
        }

        private bool _stopTimer;
        public bool StopTimer
        {
            get { return _stopTimer; }
            set { SetProperty(ref _stopTimer, value); }
        }

        private bool _isCancelable;
        public bool IsCancelable
        {
            get { return _isCancelable; }
            set { SetProperty(ref _isCancelable, value); }
        }

        private bool _clarifications;
        public bool Clarifications
        {
            get { return _clarifications; }
            set { SetProperty(ref _clarifications, value); }
        }

        private bool _isReviewable;
        public bool IsReviewable
        {
            get { return _isReviewable; }
            set { SetProperty(ref _isReviewable, value); }
        }

        private bool _visibleTime;
        public bool VisibleTime
        {
            get { return _visibleTime; }
            set { SetProperty(ref _visibleTime, value); }
        }

        private bool _notVisibleTime;
        public bool NotVisibleTime
        {
            get { return _notVisibleTime; }
            set { SetProperty(ref _notVisibleTime, value); }
        }

        private int _denominationsHeight;
        public int DenominationsHeight
        {
            get { return _denominationsHeight; }
            set { SetProperty(ref _denominationsHeight, value); }
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

        private string _time;
        public string Time
        {
            get { return _time; }
            set { SetProperty(ref _time, value); }
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

        private OrderModel _currentOrder;
        public OrderModel CurrentOrder
        {
            get { return _currentOrder; }
            set { SetProperty(ref _currentOrder, value); }
        }

        private decimal _totalExchange;
        public decimal TotalExchange
        {
            get { return _totalExchange; }
            set { SetProperty(ref _totalExchange, value); }
        }

        private OrderDetailModel[] _orderDetails;
        public OrderDetailModel[] OrderDetails
        {
            get { return _orderDetails; }
            set { SetProperty(ref _orderDetails, value); }
        }
        string _telMorrexss;
        public string TelMorrexss { get => _telMorrexss; set => SetProperty(ref _telMorrexss, value); }

        Position _position;
        public Position Position { get => _position; set => SetProperty(ref _position, value); }


        public CircledMap _MyMap;
        public CircledMap MyMap { get => _MyMap; set => SetProperty(ref _MyMap, value); }

        #endregion

        #region Commands
        public DelegateCommand ToDetailCommand { get; set; }
        public DelegateCommand CancelOrderCommand { get; set; }
        public DelegateCommand ReviewOrderCommand { get; set; }
        public DelegateCommand OpenTelMobileCommand { get; private set; }
        public DelegateCommand OpenGmailCommand { get; private set; }
        public DelegateCommand OpenTelCommand { get; private set; }
        public Command BackkCommand { get; }


        #endregion

        [Obsolete]
        public OrderDetailPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            ToDetailCommand = new DelegateCommand(ToDetail);
            CancelOrderCommand = new DelegateCommand(CancelOrder);
            ReviewOrderCommand = new DelegateCommand(ReviewOrder);
            OpenTelMobileCommand = new DelegateCommand(async () => await OpenWhatsApp());
            OpenGmailCommand = new DelegateCommand(OpenGmail);
            BackkCommand = new Command(async () => { await PopToRoot(); });

            TelMorrexss = "6622108990";
            OpenTelCommand = new DelegateCommand(OpenTel);

        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                base.OnNavigatedTo(parameters);
                if (parameters["model"] is OrderModel model)
                {
                    using (UserDialogs.Instance.Loading("Cargando..."))
                    {
                        IsEditing = true;
                        StopTimer = false;

                        // VA POR LA ORDEN ACTUAL
                        CurrentOrder = await HttpService.GetOrder(model.DeliveryId);
                        TotalExchange = CurrentOrder.TotalExchange;

                        string[] StatusCancelable = { Constants.NewOrderStatus, Constants.AcceptedOrderStatus };
                        IsCancelable = StatusCancelable.Contains(CurrentOrder.Status);

                        VisibleTime = !(CurrentOrder.Driver is null) & CurrentOrder.Status == Constants.AcceptedOrderStatus;
                        NotVisibleTime = CurrentOrder.Status != Constants.AcceptedOrderStatus;

                        Clarifications = CurrentOrder.Status == Constants.DeliveredOrderStatus || CurrentOrder.Status == Constants.CanceledByUserdOrderStatus || CurrentOrder.Status == Constants.CanceledByDriverOrderStatus;

                        IsReviewable = CurrentOrder.Status == Constants.DeliveredOrderStatus && CurrentOrder.Review == null;

                        OrderDetails = CurrentOrder.Details.Where(w => w.Quantity > 0).ToArray();
                        DenominationsHeight = OrderDetails.Count() * 45;

                        ShowAcceptedTime = CurrentOrder.Status == Constants.DeliveredOrderStatus || CurrentOrder.Status == Constants.CanceledByUserdOrderStatus || CurrentOrder.Status == Constants.CanceledByDriverOrderStatus;
                        ShowDeliveredTime = CurrentOrder.Status == Constants.DeliveredOrderStatus;

                        Status = CurrentOrder.Status;
                        Time = CurrentOrder.EstimatedDelivery;

                        SetListener();
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
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        async Task OpenWhatsApp ()
        {
            try
            {
                Chat.Open("+52 66 21 69 41 67", "Aclaraciones");
            }
            catch (Exception ex)
            {
                await PopUp("¡Error!", ex.Message, "Aceptar");
            }
        }

        void OpenGmail()
        {
            Launcher.OpenAsync(new Uri($"mailto:soporte@morrexss.com"));

        }
        public void OpenTel()
        {
            try
            {
                PhoneDialer.Open(TelMorrexss);
            }
            catch (FeatureNotSupportedException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
            StopTimer = true;
        }

        void SetListener()
        {
            var client = HttpService.GetCurrentUser();
            if (Status == Constants.AcceptedOrderStatus && client.Roles.Contains("User"))
            {
                Device.StartTimer(TimeSpan.FromSeconds(30), () =>
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Client: Updating driver location."));
                    var updatedOrderTask = Task.Run(async () => await HttpService.GetOrder(CurrentOrder.DeliveryId));
                    updatedOrderTask.Wait();
                    MyMap.Circle.Radius = updatedOrderTask.Result.EstimatedDistanceInMeters;
                    Status = updatedOrderTask.Result.Status;
                    IsEditing = Status == Constants.AcceptedOrderStatus;
                    return IsEditing & !StopTimer;
                });
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

                double distanceMeters = CurrentOrder?.EstimatedDistanceInMeters ?? 0;
                MyMap.Circle.Position = positionMap;
                MyMap.Circle.Radius = distanceMeters;
            }

            MyMap.MoveToRegion(new MapSpan(positionMap, 0.01, 0.01));
        }

        //async void ToDetail() =>
        //    await Navigate("DriverDetailPage", new NavigationParameters { { "model", CurrentOrder } });


        async void ToDetail()
        {
            await NavigationService.NavigateAsync("DriverDetailPage", new NavigationParameters { { "model", CurrentOrder } });

        }


        async void CancelOrder()
        {
            await NavigationService.NavigateAsync("CancelOrderClient", new NavigationParameters { { "model", CurrentOrder } });

        }

        async void ReviewOrder()
        {
            await Navigate("ReviewDelivery", new NavigationParameters { { "model", CurrentOrder } });
            //await PopUp("Confirmar", $"Calificar pedido", "Aceptar", null, "Cancelar", null);
        }
    }
}
