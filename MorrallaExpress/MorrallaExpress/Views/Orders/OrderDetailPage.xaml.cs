using MorrallaExpress.Controls;
using MorrallaExpress.ViewModels.Orders;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace MorrallaExpress.Views.Orders
{
    public partial class OrderDetailPage : ContentPage
    {
        public Position Position { get; set; }

        public OrderDetailPage()
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

            if (Device.RuntimePlatform == Device.Android)
            {
                CircleButton.WidthRequest = 21;
                CircleButton.CornerRadius = 25;
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext is OrderDetailPageViewModel vm)
            {
                vm.MyMap = MainMap;
                MainMap.Circle = new CustomCircle
                {
                    Position = new Position(23.2935962, -111.6477312),
                    Radius = 0
                };
                MainMap.MoveToRegion(new MapSpan(MainMap.Circle.Position, 0.01, 0.01));

            }
        }
    }
}
