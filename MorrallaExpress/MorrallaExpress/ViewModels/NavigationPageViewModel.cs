using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.ViewModels
{
    public class NavigationPageViewModel : ViewModelBase
    {
        public NavigationPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }
    }
}
