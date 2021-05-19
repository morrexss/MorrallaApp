using MorrallaExpress.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNaviRenderer))]
namespace MorrallaExpress.iOS.CustomRenderers
{
    public class CustomNaviRenderer : NavigationRenderer
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