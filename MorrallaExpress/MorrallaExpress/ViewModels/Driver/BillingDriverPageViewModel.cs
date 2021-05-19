using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using MorrallaExpress.Services;
using MorrallaExpress.Validations;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MorrallaExpress.ViewModels
{
    public class BillingDriverPageViewModel : ViewModelBase
    {
        public UserSession _driver = new UserSession();
        public UserSession Driver
        {
            get => _driver;
            set => SetProperty(ref _driver, value);
        }

        public BillingDriverPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            Driver = HttpService.GetCurrentUser();
        }
    }
}