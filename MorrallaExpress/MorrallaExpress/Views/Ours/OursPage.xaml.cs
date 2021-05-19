using Prism.Navigation;
using Xamarin.Forms;

namespace MorrallaExpress.Views.Ours
{
    public partial class OursPage : ContentPage
    {
        public OursPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, true);
        }
    }
}
