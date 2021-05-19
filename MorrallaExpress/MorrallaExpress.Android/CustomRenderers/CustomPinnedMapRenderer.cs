using System;
using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Maps;
using Android.Gms.Maps;
using System.ComponentModel;
using Xamarin.Forms.Maps.Android;
using MobileApp.Droid.Renderers;
using MorrallaExpress.Controls;

[assembly: ExportRenderer(typeof(CustomPinnedMap), typeof(CustomMapRenderer))]
namespace MobileApp.Droid.Renderers
{
    /// Renderer for the xamarin map.
    /// Enable user to get a position by taping on the map.
    ///
    public class CustomMapRenderer : MapRenderer, IOnMapReadyCallback
    {
        // We use a native google map for Android
        private GoogleMap _map;
        public CustomMapRenderer(Context context) : base(context)
        {

        }

        protected override void OnMapReady(GoogleMap googleMap)
        {
            base.OnMapReady(googleMap);

            _map = googleMap;
            _map.MyLocationEnabled = true;
            if (_map != null)
                _map.MapClick += googleMap_MapClick;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (_map != null)
                _map.MapClick -= googleMap_MapClick;

            if (Control != null)
                ((MapView)Control).GetMapAsync(this);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (_map != null)
                _map.MapClick -= googleMap_MapClick;
            if (_map != null)
                _map.MapClick += googleMap_MapClick;
        }

        private void googleMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            Map.Pins.Clear();
            double latitude = Math.Round(e.Point.Latitude, 6);
            double longitude = Math.Round(e.Point.Longitude, 6);
            Position Position = new Position(latitude, longitude);
            Map.Pins.Add(new Pin { Label = "Localización de mi sucursal", Position = Position });
        }

    }
}