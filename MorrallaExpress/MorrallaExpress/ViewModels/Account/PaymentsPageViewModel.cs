using MorrallaExpress.Models.DTOs;
using MorrallaExpress.Validations;
using MorrallaExpress.Validations.Rules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using MorrallaExpress.Helpers.Interfaces;
using System.Collections.ObjectModel;
using MorrallaExpress.Extensions;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace MorrallaExpress.ViewModels.Account
{
    public class PaymentsPageViewModel : ViewModelBase
    {
        #region Props
        public bool _emptyView = false;
        public bool EmptyView { get => _emptyView; set => SetProperty(ref _emptyView, value); }

        public bool _lista = true;
        public bool Lista { get => _lista; set => SetProperty(ref _lista, value); }

        bool _isListRefreshing = false;
        public bool IsListRefreshing
        {
            get => _isListRefreshing;
            set => SetProperty(ref _isListRefreshing, value);
        }

        #endregion

        #region Commands
        public DelegateCommand<PaymentMethodModel> ToDetailCommand { get; set; }
        public DelegateCommand RefreshListCommand { get; set; }
        #endregion
        public ObservableCollection<PaymentMethodModel> PaymentMethods { get; set; } = new ObservableCollection<PaymentMethodModel>();
        
        public PaymentsPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            Title = "Métodos de pago";
            PaymentMethods = httpService.GetCurrentPaymentMethods().ToObservableCollection();
            ToDetailCommand = new DelegateCommand<PaymentMethodModel>(async (model) => await ToDetail(model));
            RefreshListCommand = new DelegateCommand(RefreshList);
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            await RefreshListIntern();
        }

        async void RefreshList()
        {
            await RefreshListIntern();
        }

        async Task RefreshListIntern()
        {
            IsListRefreshing = true;
            var paymentMethods = await HttpService.GetPaymentMethods(true);
            PaymentMethods.Clear();
            foreach (var paymentMethod in paymentMethods)
            {
                paymentMethod.DeleteCommand = new DelegateCommand<PaymentMethodModel>(Delete);
                PaymentMethods.Add(paymentMethod);
            }
            IsListRefreshing = false;

            if (PaymentMethods.Count == 0)
            {
                EmptyView = true;
                Lista = false;
            }
            else
            {
                EmptyView = false;
                Lista = true;
            }
        }
        async void Delete(PaymentMethodModel modelo)
        {
            var deleteCmd = new DelegateCommand(async () => {
                using (UserDialogs.Instance.Loading("Cargando..."))
                    await HttpService.DeletePaymentMethod(modelo.PaymentMethodId);
                await RefreshListIntern();
                //await NavigateBack();
            });

            await PopUp("Advertencia", $"Estás seguro que deseas eliminar {modelo.MaskedCard}?",
                "Aceptar", deleteCmd, "Cancelar", null);
        }

        async Task ToDetail(PaymentMethodModel model) =>
            await Navigate("PaymentsDetailPage", new NavigationParameters { { "model", model } });
    }
}
