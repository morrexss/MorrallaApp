using Prism.Navigation;
using Xamarin.Forms;

namespace MorrallaExpress.Views.Ours
{
    public partial class ContactUsPage : ContentPage
    {
        public ContactUsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, true);
        }
    }
}
