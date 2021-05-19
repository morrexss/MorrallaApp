using Xamarin.Forms;

namespace MorrallaExpress.Views
{
    public partial class CancelOrderClient : ContentPage
    {
        public CancelOrderClient()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, true);
            if (Device.RuntimePlatform == Device.iOS)
            {
                StackPicker.Padding = 14;
            }
        }
    }
}
