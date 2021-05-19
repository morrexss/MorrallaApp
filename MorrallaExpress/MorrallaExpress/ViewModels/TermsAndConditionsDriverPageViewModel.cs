using MorrallaExpress.Helpers.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MorrallaExpress.ViewModels
{
    public class TermsAndConditionsDriverPageViewModel : ViewModelBase
    {
        public TermsAndConditionsDriverPageViewModel(INavigationService navigationService, IHttpService httpService, IEventAggregator eventService) : base(navigationService, httpService, eventService)
        {
        }
    }
}
