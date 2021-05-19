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
    public class HistoryOrdersDriverPageViewModel : ViewModelBase
    {
        public ObservableCollection<OrderModel> Orders { get; set; } = new ObservableCollection<OrderModel>();
        public DelegateCommand ToDetailCommand { get; set; }
        public DelegateCommand RefreshListCommand { get; set; }

        #region Props
        private bool _forceLoad = true;
        public bool _emptyView = false;
        public bool EmptyView { get => _emptyView; set => SetProperty(ref _emptyView, value); }

        public bool _lista = true;
        public bool Lista { get => _lista; set => SetProperty(ref _lista, value); }

        public bool _isRefreshing = true;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        public OrderModel _SelectItem = new OrderModel();
        public OrderModel SelectItem { get => _SelectItem; set => SetProperty(ref _SelectItem, value); }

        #endregion
        public HistoryOrdersDriverPageViewModel(INavigationService navigationService, IHttpService httpService) 
            : base(navigationService, httpService)
        {
            Title = "Historial";
            ToDetailCommand = new DelegateCommand(ToDetail);
            RefreshListCommand = new DelegateCommand(RefreshList);
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters["force"] is bool force)
                _forceLoad = force;
            else
                _forceLoad = Orders.Count == 0;
            await Task.Run(RefreshListIntern);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
           //SelectItem = null;
        }

        private async void RefreshList()
        {
            _forceLoad = true;
            await Task.Run(RefreshListIntern);
        }

        async void RefreshListIntern()
        {
            IsRefreshing = true;
            var orders = await HttpService.GetOrdersDriverHistory(true);
            var active = HttpService.GetCurrentActiveOrders();
            Orders.Clear();
            foreach (var item in orders)
                Orders.Add(item);
            if (Orders.Count == 0)
            {
                EmptyView = true;
                Lista = false;
            }
            else
            {
                EmptyView = false;
                Lista = true;
            }
            IsRefreshing = false;
        }

        async void ToDetail() =>
            await Navigate("OrderDetailDriverPage", new NavigationParameters { { "model", SelectItem } });
    }
}
