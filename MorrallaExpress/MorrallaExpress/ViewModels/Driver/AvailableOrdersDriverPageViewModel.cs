using MorrallaExpress.Extensions;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using MorrallaExpress.Models.DTOs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MorrallaExpress.ViewModels
{
    public class AvailableOrdersDriverPageViewModel : ViewModelBase
    {
        public ObservableCollection<OrderModel> Orders { get; set; } = new ObservableCollection<OrderModel>();
        public DelegateCommand ToDetailCommand { get; set; }
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand ProfileCommand { get; set; }


        #region Props
        private bool _forceLoad = true;

        public bool _emptyView = false;
        public bool EmptyView { get => _emptyView; set => SetProperty(ref _emptyView, value); }

        public bool _noServiceView = false;
        public bool NoServiceView { get => _noServiceView; set => SetProperty(ref _noServiceView, value); }

        public bool _lista = true;
        public bool Lista { get => _lista; set => SetProperty(ref _lista, value); }
        public bool _isRefreshing = true;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }


        public OrderModel _SelectItem = new OrderModel();
        public OrderModel SelectItem { get => _SelectItem; set => SetProperty(ref _SelectItem, value); }

        #endregion
        public AvailableOrdersDriverPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            Title = "Disponibles";
            ToDetailCommand = new DelegateCommand(ToDetail);
            RefreshCommand = new DelegateCommand(RefreshList);
            //ProfileCommand = new DelegateCommand(ToProfile);
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var driverService = HttpService.GetDriverService();
            if (!driverService)
            {
                NoServiceView = true;
                Lista = false;
            }
            else
            {
                if (parameters["force"] is bool force)
                    _forceLoad = force;
                else
                    _forceLoad = Orders.Count == 0;
                NoServiceView = false;
                Lista = true;
                await Task.Run(RefreshListIntern);
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }

        private async void RefreshList()
        {
            _forceLoad = true;
            await Task.Run(RefreshListIntern);
        }

        private async void RefreshListIntern()
        {
            IsRefreshing = true;
            var orders = await HttpService.GetOrdersDriverAvailable(_forceLoad);
            Orders.Clear();
            foreach (var order in orders)
                Orders.Add(order);
            if (Orders.Count == 0)
            {
                EmptyView = true;
                Lista = false;
                NoServiceView = false;
            }
            else
            {
                EmptyView = false;
                Lista = true;
                NoServiceView = false;
            }

            IsRefreshing = false;
        }

        async void ToDetail() =>
            await Navigate("OrderDetailDriverPage", new NavigationParameters { { "model", SelectItem } });
        //async void ToProfile() =>
        //    await Navigate("ProfileDriverPage");
    }
}
