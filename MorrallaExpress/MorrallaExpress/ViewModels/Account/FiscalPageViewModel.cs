using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using MorrallaExpress.Validations;
using MorrallaExpress.Validations.Rules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace MorrallaExpress.ViewModels.Account
{
    public class FiscalPageViewModel : ViewModelBase
    {
        public DelegateCommand UpdateInformationCommand { get; set; }

        public UserSession _User = new UserSession();
        public UserSession User
        {
            get => _User;
            set => SetProperty(ref _User, value);
        }

        public FiscalPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            UpdateInformationCommand = new DelegateCommand(Update);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            User = new UserSession();
            User = HttpService.GetCurrentUser();
        }

        public async void Update()
        {
            await NavigationService.NavigateAsync("EditFiscalPage");
        }
    } 
}
