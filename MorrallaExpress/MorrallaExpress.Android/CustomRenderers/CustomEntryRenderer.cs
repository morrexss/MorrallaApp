using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MorrallaExpress.Controls;
using MorrallaExpress.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace MorrallaExpress.Droid.CustomRenderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context)
        { }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null || e.NewElement == null) return;

            UpdateBorders();
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control == null) return;

            if (e.PropertyName == CustomEntry.IsBorderErrorVisibleProperty.PropertyName)
                UpdateBorders();

            if (e.PropertyName.Equals(CustomEntry.ShapeColorProperty.PropertyName) || e.PropertyName.Equals(CustomEntry.BorderColorProperty.PropertyName))
            {
                UpdateBorders();
            }
        }

        void UpdateBorders()
        {
            var customEntry = this.Element as CustomEntry;
            GradientDrawable shape = new GradientDrawable();
            shape.SetShape(ShapeType.Rectangle);
            shape.SetStroke(1, customEntry.BorderColor.ToAndroid());
            shape.SetCornerRadius(customEntry.CornerRadius);
            shape.SetAlpha(240);
            shape.SetColor(customEntry.ShapeColor.ToAndroid());

            if (customEntry.IsBorderErrorVisible)
            {
                shape.SetStroke(3, customEntry.BorderErrorColor.ToAndroid());
            }
            else
            {
                shape.SetStroke(3, customEntry.BorderColor.ToAndroid());
                this.Control.SetBackground(shape);
            }
            Control.SetPadding(60,0,0,0);
            Control.Gravity = GravityFlags.CenterVertical;
            Control.SetBackground(shape);
        }

    }
}