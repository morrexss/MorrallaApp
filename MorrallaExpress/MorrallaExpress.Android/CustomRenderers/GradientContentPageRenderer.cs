using System;
using Android.Content;
using MorrallaExpress.Controls;
using MorrallaExpress.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GradientContentPage), typeof(GradientContentPageRenderer))]
namespace MorrallaExpress.Droid.CustomRenderers
{
    public class GradientContentPageRenderer : PageRenderer
    {
        private Color BackgroundStartColor { get; set; }
        private Color BackgroundEndColor { get; set; }
        private float Radius { get; set; }
        public GradientContentPageRenderer(Context ex) : base(ex) { }

        protected override void DispatchDraw(global::Android.Graphics.Canvas canvas)
        {


            var gradient = new Android.Graphics.RadialGradient(Width / 2, Height / 2, Radius,
                BackgroundStartColor.ToAndroid(), BackgroundEndColor.ToAndroid(), Android.Graphics.Shader.TileMode.Clamp);
            // hacer switch con gradient vert, horiz, rad

            var paint = new Android.Graphics.Paint()
            {
                Dither = true,
            };
            paint.SetShader(gradient);
            canvas.DrawPaint(paint);
            base.DispatchDraw(canvas);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
                return;
            try
            {
                var page = e.NewElement as GradientContentPage;
                BackgroundStartColor = page.BackgroundStartColor;
                BackgroundEndColor = page.BackgroundEndColor;
                Radius = page.GradientRadius;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"ERROR:", ex.Message);
            }
        }
    }
}