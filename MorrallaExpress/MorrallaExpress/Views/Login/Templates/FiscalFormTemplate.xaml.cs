using MorrallaExpress.ViewModels.Login;
using Xamarin.Forms;

namespace MorrallaExpress.Views.Login.Templates
{
    public partial class FiscalFormTemplate : ScrollView
    {
        public static readonly BindableProperty ParentContextProperty =
           BindableProperty.Create(nameof(ParentContext), typeof(object), typeof(FiscalFormTemplate), null, propertyChanged: OnParentContextPropertyChanged);

        public FiscalFormTemplate()
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
                (bindable as FiscalFormTemplate).ParentContext = newValue;
        }
    }
}
