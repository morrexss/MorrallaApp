using System;
using MorrallaExpress.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ListView), typeof(ListViewCustomRenderer))]
namespace MorrallaExpress.iOS.CustomRenderers
{
    public class ListViewCustomRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;

            Control.TableFooterView = new UIView();
        }
    }
}
