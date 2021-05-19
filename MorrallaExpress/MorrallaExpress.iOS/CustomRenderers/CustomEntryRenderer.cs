using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using MorrallaExpress.Controls;
using MorrallaExpress.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace MorrallaExpress.iOS.CustomRenderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var customEntry = (CustomEntry)Element; 

            if (Control == null || Element == null)
                return;
            Control.Layer.CornerRadius = customEntry.CornerRadius / 4;
            //Control.Layer.BorderWidth = 10;
            Control.BorderStyle = UITextBorderStyle.None;
            Control.Layer.BackgroundColor = customEntry.ShapeColor.ToCGColor();
            Control.Layer.BorderColor = customEntry.BorderColor.ToCGColor(); // Xamarin.Forms.Color.White.ToCGColor();
            //float pad = customEntry.HorizontalPadding / 3.3f;
            Control.LeftView = new UIView(new CGRect(0, 0,30, 0));
            Control.LeftViewMode = UITextFieldViewMode.Always;

            if (e.PropertyName == CustomEntry.IsBorderErrorVisibleProperty.PropertyName)
                Control.Layer.BorderColor = customEntry.IsBorderErrorVisible ?
                    customEntry.BorderErrorColor.ToCGColor() : Color.Transparent.ToCGColor();
        }
    }

}