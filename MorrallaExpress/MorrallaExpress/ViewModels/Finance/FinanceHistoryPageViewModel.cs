using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models.DTOs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MorrallaExpress.Models;
using MorrallaExpress.Helpers;

namespace MorrallaExpress.ViewModels.Finance
{
    public class FinanceHistoryPageViewModel : ViewModelBase
    {
        public ObservableCollection<object> Orders { get; set; } = new ObservableCollection<object>();
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand DateModalCommand { get; set; }

        public List<OrderModel> History = new List<OrderModel>();

        private bool _forceLoad = true;

        public bool _emptyView = false;
        public bool EmptyView { get => _emptyView; set => SetProperty(ref _emptyView, value); }

        public bool _isRefreshing = true;
        public bool IsRefreshing { get => _isRefreshing; set => SetProperty(ref _isRefreshing, value); }

        public string _search;
        public string Search { get => _search; set => SetProperty(ref _search, value); }

        public bool _lista = true;
        public bool Lista { get => _lista; set => SetProperty(ref _lista, value); }

        public DateTime _startDate;
        public DateTime StartDate { get => _startDate; set => SetProperty(ref _startDate, value, FilterDate); }

        public DateTime _endDate;
        public DateTime EndDate { get => _endDate; set => SetProperty(ref _endDate, value, FilterDate); }

        public decimal _totalDriverComission;
        public decimal TotalDriverComission { get => _totalDriverComission; set => SetProperty(ref _totalDriverComission, value); }

        public bool _DateModal = false;
        public bool DateModal
        {
            get { return _DateModal; }
            set { SetProperty(ref _DateModal, value); }
        }

        private string _fechaCorte;
        public string FechaCorte { get => _fechaCorte; set => SetProperty(ref _fechaCorte, value); }

        private string _pago;
        public string Pago { get => _pago; set => SetProperty(ref _pago, value); }
        public DelegateCommand OnSearchChangedCommand { get; set; }
        public FinanceHistoryPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            RefreshCommand = new DelegateCommand(RefreshList);
            OnSearchChangedCommand = new DelegateCommand(OnSearchChanged);
            EndDate = DateTime.Now;
            StartDate = EndDate.AddMonths(-1);
            DateModalCommand = new DelegateCommand(MorrexssModal);
        }

        public void FilterDate()
        {
            var OrdersFilterDateStart = History.Where(n => n.DeliveredDate >= StartDate && n.DeliveredDate <= EndDate.AddDays(1)).OrderByDescending(n => n.DeliveredDate);
            TotalDriverComission = 0;
            if (OrdersFilterDateStart.Count() > 0)
            {
                Orders.Clear();
                foreach (var item in OrdersFilterDateStart)
                {
                    Orders.Add(item);
                    TotalDriverComission += item.DriverComission;
                }
                IsRefreshing = false;
            }
        }

        public async void OnSearchChanged()
        {
            if (string.IsNullOrEmpty(Search))
                await Task.Run(RefreshListIntern);
            var filtered = History.Where(Y => Y.DeliveryId.ToString().Contains(Search)).ToList();
            TotalDriverComission = 0;
            if (filtered.Count() > 0)
            {
                Orders.Clear();
                foreach (var item in filtered)
                {
                    Orders.Add(item);
                    TotalDriverComission += item.DriverComission;
                }
            }
            IsRefreshing = false;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
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
           
            History = (await HttpService.GetOrdersDriverHistory(_forceLoad)).ToList();
            History = History.Where(x => x.Status == Constants.DeliveredOrderStatus && x.Paid == true && x.Payout != null).ToList();
            Orders.Clear();

            var dates = History.GroupBy(h => h?.Payout?.Date).ToArray();
            TotalDriverComission = 0;

            foreach (var group in dates)
            {
                decimal count = 0;
                foreach (var order in group)
                {

                    Orders.Add(order);
                    if (order.Paid)
                    {
                        count += order.DriverComission;
                        TotalDriverComission += order.DriverComission;
                    }
                }
                if (group.Count() > 0)
                    Orders.Add(new TemplateListSubTotalModel
                    {
                        Date = group.Key?.ToString("dd/MM/yyyy"),
                        SubTotal = string.Format("{0:C}", count),
                        Tapped = new DelegateCommand<TemplateListSubTotalModel>(TappedCommand)
                    });
            }
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

        public async void TappedCommand(TemplateListSubTotalModel model)
        {
            DateModal = !DateModal;
            FechaCorte = model.Date;
            Pago = model.SubTotal;
        }
        public async void MorrexssModal()
        {
            DateModal = !DateModal;
            FechaCorte = null;
            Pago = null;
        }
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);
        }
    }
}
