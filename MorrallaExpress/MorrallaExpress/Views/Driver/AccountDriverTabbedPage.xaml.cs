using MorrallaExpress.ViewModels;
using Prism.Navigation;
using Xamarin.Forms;

namespace MorrallaExpress.Views.Driver
{
    public partial class AccountDriverTabbedPage : TabbedPage, INavigatedAware
    {
        public AccountDriverTabbedPage()
        {
            InitializeComponent();
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            // You need to manually raise OnNavigatedTo in child pages on 1st time navigation, this is necessary to set Busy & CanNavigate in the base class
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                // Prism always raises OnNavigatedTo on 1st tabbed page so this prevents the first tab being initialised twice
                if (Children.Count == 1)
                {
                    return;
                }
                for (var pageIndex = 1; pageIndex < Children.Count; pageIndex++)
                {
                    var page = Children[pageIndex];
                    if (page?.BindingContext is INavigationAware tmp)
                    {
                        (BindingContext as AccountDriverTabbedPageViewModel).Title = (page?.BindingContext as ViewModelBase).Title;
                        tmp.OnNavigatedTo(parameters);
                    }
                }
            }
        }
    }
}
