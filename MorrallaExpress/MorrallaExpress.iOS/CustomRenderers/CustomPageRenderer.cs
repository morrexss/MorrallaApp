using MorrallaExpress.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(CustomPageRenderer))]
namespace MorrallaExpress.iOS.Renderers
{
    public class CustomPageRenderer : PageRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
                return;
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
        }
    }
}