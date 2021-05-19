using Xamarin.Essentials;
using Xamarin.Forms;

namespace MorrallaExpress.Views.Account
{
    public partial class ChangePasswordPage : ContentPage
    {
        public ChangePasswordPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");
            var WidthPassword = DeviceDisplay.MainDisplayInfo.Height < 1800 ? 355 : 300;
            WidthRequestPassword.WidthRequest = WidthPassword;
        }
    }
}
