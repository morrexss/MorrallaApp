using Prism.Navigation;
using Xamarin.Forms;

namespace MorrallaExpress.Views.Legal
{
    public partial class LegalPage : ContentPage
    {
        public LegalPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, true);
        }
    }
}
