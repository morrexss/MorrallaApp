using MorrallaExpress.ViewModels.Login;
using Xamarin.Forms;

namespace MorrallaExpress.Views.Login.Templates
{
    public partial class LocationFormTemplate : ScrollView
    {
        public static readonly BindableProperty ParentContextProperty =
           BindableProperty.Create(nameof(ParentContext), typeof(object), typeof(LocationFormTemplate), null, propertyChanged: OnParentContextPropertyChanged);

        public LocationFormTemplate()
        {
            InitializeComponent();
        }

        public object ParentContext
        {
            get => GetValue(ParentContextProperty);
            set => SetValue(ParentContextProperty, value);
        }

        private static void OnParentContextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != oldValue && newValue != null)
                (bindable as LocationFormTemplate).ParentContext = newValue;
        }
    }
}
