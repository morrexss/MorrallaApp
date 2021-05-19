using MorrallaExpress.Helpers.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MorrallaExpress.ViewModels
{
    public class AccountDriverTabbedPageViewModel : ViewModelBase
    {

        public AccountDriverTabbedPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
        }
    }
}
