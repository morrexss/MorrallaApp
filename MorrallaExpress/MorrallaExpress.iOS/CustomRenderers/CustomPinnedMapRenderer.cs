using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using MorrallaExpress.Controls;
using Xamarin.Forms.Maps.iOS;
using MorrallaExpress.iOS.Renderers;
using MapKit;
using Xamarin.Forms.Maps;

[assembly: ExportRenderer(typeof(CustomPinnedMap), typeof(CustomPinnedMapRenderer))]
namespace MorrallaExpress.iOS.Renderers
{
    public class CustomPinnedMapRenderer : MapRenderer
    {

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var formsMap = (CustomPinnedMap)e.OldElement;
                formsMap.MapClicked -= Map_MapClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomPinnedMap)e.NewElement;
                formsMap.MapClicked += Map_MapClick;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var formsMap = (CustomPinnedMap)sender;

            formsMap.MapClicked += Map_MapClick;
        }

        private void Map_MapClick(object sender, MapClickedEventArgs e)
        {
            var formsMap = (CustomPinnedMap)sender;
            formsMap.Pins.Clear();
            double latitude = Math.Round(e.Position.Latitude, 6);
            double longitude = Math.Round(e.Position.Longitude, 6);
            Position Position = new Position(latitude, longitude);
            formsMap.Pins.Add(new Pin { Label = "Localización de mi barco", Position = Position });
        }
    }
}