using MorrallaExpress.ViewModels.Account;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace MorrallaExpress.Views.Account
{
    public partial class LocationDetailPage : ContentPage
    {
        public Position Position { get; set; }

        public LocationDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is LocationDetailPageViewModel vm)
            {
                vm.MyMap = MainMap;
                //Position locationPosition = vm.Position;
                //string locationName = vm.Name.Value;

                //if (locationPosition != null && locationPosition.Latitude != 0 && locationPosition.Longitude != 0)
                //{
                //    Pin pin = new Pin
                //    {
                //        Label = locationName,
                //        Type = PinType.Place,
                //        Position = locationPosition
                //    };
                //    MainMap.Pins.Add(pin);
                //}
                //// iniciamos el mapa en el PIN anterior o posición actual
                //MainMap.MoveToRegion(new MapSpan(locationPosition, 0.01, 0.01));
            }
        }

    }
}
