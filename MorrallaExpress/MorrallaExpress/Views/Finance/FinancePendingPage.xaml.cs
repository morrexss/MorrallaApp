using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MorrallaExpress.Views.Finance
{
    public partial class FinancePendingPage : ContentPage
    {
        public FinancePendingPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, true);
        }
    }
}
