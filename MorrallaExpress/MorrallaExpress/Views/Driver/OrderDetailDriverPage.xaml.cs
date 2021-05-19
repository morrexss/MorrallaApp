using MorrallaExpress.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace MorrallaExpress.Views.Driver
{
    public partial class OrderDetailDriverPage : ContentPage
    {
        public Position Position { get; set; }

        public OrderDetailDriverPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            if (Device.RuntimePlatform == Device.iOS)
            {
                var headerH = DeviceDisplay.MainDisplayInfo.Height > 1334 ? 87 : 60;
                var ImgH = DeviceDisplay.MainDisplayInfo.Height > 1334 ? 40 : 60;
                var MarginH = DeviceDisplay.MainDisplayInfo.Height > 1334 ? 30 : 0;
                HeaderRow.Height = headerH;
                Im.HeightRequest = ImgH;
                Header.Margin = new Thickness(0, MarginH, 0, -7);
                //HeaderRow.Margin = new Thickness(15, martintop, 0, 0);
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext is OrderDetailDriverPageViewModel vm)
            {
                vm.MyMap = MainMap;
            }
        }
    }
}
