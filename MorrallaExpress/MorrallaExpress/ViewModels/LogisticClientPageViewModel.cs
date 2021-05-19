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
    public class LogisticClientPageViewModel : ViewModelBase
    {
        public LogisticClientPageViewModel(INavigationService navigationService, IHttpService httpService, IEventAggregator eventService) : base(navigationService, httpService, eventService)
        {
        }
    }
}
