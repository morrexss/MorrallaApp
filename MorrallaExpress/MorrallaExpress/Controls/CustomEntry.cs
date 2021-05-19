using Xamarin.Forms;

namespace MorrallaExpress.Controls
{
    public class CustomEntry : Entry
    {
        public static readonly BindableProperty IsBorderErrorVisibleProperty =
            BindableProperty.Create(nameof(IsBorderErrorVisible), typeof(bool), typeof(CustomEntry), false, BindingMode.TwoWay);

        public bool IsBorderErrorVisible
        {
            get { return (bool)GetValue(IsBorderErrorVisibleProperty); }
            set
            {
                SetValue(IsBorderErrorVisibleProperty, value);
            }
        }

        public static readonly BindableProperty BorderErrorColorProperty =
            BindableProperty.Create(nameof(BorderErrorColor), typeof(Color), typeof(CustomEntry), Color.Transparent, BindingMode.TwoWay);

        public Color BorderErrorColor
        {
            get { return (Color)GetValue(BorderErrorColorProperty); }
            set
            {
                SetValue(BorderErrorColorProperty, value);
            }
        }

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(CustomEntry), Color.Transparent, BindingMode.TwoWay);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set
            {
                SetValue(BorderColorProperty, value);
            }
        }

        public static readonly BindableProperty ShapeColorProperty =
            BindableProperty.Create(nameof(ShapeColor), typeof(Color), typeof(CustomEntry), Color.FromRgb(245,246,248), BindingMode.TwoWay);

        public Color ShapeColor
        {
            get { return (Color)GetValue(ShapeColorProperty); }
            set
            {
                SetValue(ShapeColorProperty, value);
            }
        }

        public static readonly BindableProperty ErrorTextProperty =
        BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(CustomEntry), string.Empty);

        public string ErrorText
        {
            get { return (string)GetValue(ErrorTextProperty); }
            set
            {
                SetValue(ErrorTextProperty, value);
            }
        }

        public int CornerRadius { get; set; }

        //public static readonly BindableProperty CornerRadiusProperty =
        //BindableProperty.Create(nameof(CornerRadius), typeof(float), typeof(CustomEntry), 24.5f);
        //public float CornerRadius
        //{
        //    get { return (float)GetValue(CornerRadiusProperty); }
        //    set
        //    {
        //        SetValue(CornerRadiusProperty, value);
        //    }
        //}


        //public static readonly BindableProperty HorizontalPaddingProperty =
        //BindableProperty.Create(nameof(HorizontalPadding), typeof(int), typeof(CustomEntry), 0);
        //public int HorizontalPadding
        //{
        //    get { return (int)GetValue(HorizontalPaddingProperty); }
        //    set
        //    {
        //        SetValue(HorizontalPaddingProperty, value);
        //    }
        //}

    }
}
