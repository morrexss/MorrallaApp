using Prism.Navigation;
using Xamarin.Forms;

namespace MorrallaExpress.Views.Ours
{
    public partial class LogisticPage : ContentPage
    {
        public LogisticPage()
        {
            try
            {
                InitializeComponent();
                NavigationPage.SetHasBackButton(this, true);
            }
            catch (System.Exception e)
            {
            }
            
        }

    }
}
