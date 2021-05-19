using Xamarin.Forms;

namespace MorrallaExpress.Views.Login.Templates
{
    public partial class BasicFormTemplate : ScrollView
    {
        public static readonly BindableProperty ParentContextProperty =
        BindableProperty.Create(nameof(ParentContext), typeof(object), typeof(BasicFormTemplate), null, propertyChanged: OnParentContextPropertyChanged);

        public BasicFormTemplate()
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
                (bindable as BasicFormTemplate).ParentContext = newValue;
        }
    }
}
