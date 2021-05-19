using Prism.Navigation;
using Xamarin.Forms;

namespace MorrallaExpress.Views.Finance
{
    public partial class FinanceTabbedPage : TabbedPage, INavigatedAware
    {
        public FinanceTabbedPage()
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
                for (var pageIndex = 0; pageIndex < Children.Count; pageIndex++)
                {
                    var page = Children[pageIndex];
                    (page?.BindingContext as INavigationAware)?.OnNavigatedTo(parameters);
                }
            }
        }
    }
}
