using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MorrallaExpress.Views.Finance
{
    public partial class FinanceHistoryPage : ContentPage
    {
        public FinanceHistoryPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, true);
           
        }
    }
}
