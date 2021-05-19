using MorrallaExpress.Events;
using MorrallaExpress.Extensions;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using MorrallaExpress.Models.DTOs;
using MorrallaExpress.Validations;
using MorrallaExpress.Validations.Rules;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MorrallaExpress.ViewModels.Orders
{
    public class ActiveOrdersPageViewModel : ViewModelBase
    {
        public ObservableCollection<OrderModel> Orders { get; set; } = new ObservableCollection<OrderModel>();
        public DelegateCommand ToDetailCommand { get; set; }
        public DelegateCommand RefreshCommand { get; set; }

        //public DelegateCommand RefreshListCommand { get; set; }

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

        public ActiveOrdersPageViewModel(INavigationService navigationService, IHttpService httpService, IEventAggregator eventService)
            : base(navigationService, httpService, eventService)
        {
            Title = "Órdenes activas";
            ToDetailCommand = new DelegateCommand(ToDetail);
            RefreshCommand = new DelegateCommand(RefreshList);

            //RefreshListCommand = new DelegateCommand(RefreshList);
            //eventService.GetEvent<OrdersLoadedEvent>().Subscribe(OnOrdersLoadedEvent, ThreadOption.UIThread);
            //IsRefreshing = true;
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

        private async void RefreshList()
        {
            _forceLoad = true;
            await Task.Run(RefreshListIntern);
        }
        
        private async void RefreshListIntern()
        {
            IsRefreshing = true;
            var orders = await HttpService.GetActiveOrders(_forceLoad);
            Orders.Clear();
            foreach (var order in orders)
                Orders.Add(order);
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
            await Navigate("OrderDetailPage", new NavigationParameters { { "model", SelectItem } });
    }
}
