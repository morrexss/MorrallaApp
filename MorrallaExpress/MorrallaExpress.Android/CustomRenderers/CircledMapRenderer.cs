using Android.Content;
using Android.Gms.Maps.Model;
using MorrallaExpress.Controls;
using MorrallaExpress.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CircledMap), typeof(CircledMapRenderer))]
namespace MorrallaExpress.Droid.CustomRenderers
{
    public class CircledMapRenderer : MapRenderer
    {
        CustomCircle circle;

        public CircledMapRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Maps.Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                var formsMap = (CircledMap)e.NewElement;
                circle = formsMap.Circle;
            }
        }

        protected override void OnMapReady(Android.Gms.Maps.GoogleMap map)
        {
            base.OnMapReady(map);
            if (!(circle is null))
            {
                var circleOptions = new CircleOptions();
                circleOptions.InvokeCenter(new LatLng(circle.Position.Latitude, circle.Position.Longitude));
                circleOptions.InvokeRadius(circle.Radius);
                circleOptions.InvokeFillColor(0X66FF0000);
                circleOptions.InvokeStrokeColor(0X66FF0000);
                circleOptions.InvokeStrokeWidth(0);

                NativeMap.AddCircle(circleOptions);
            }
        }

    }
}
