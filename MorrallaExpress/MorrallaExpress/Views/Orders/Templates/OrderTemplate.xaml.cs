using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MorrallaExpress.Views.Orders.Templates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderTemplate : ViewCell
    {
        public OrderTemplate()
        {
            InitializeComponent();
        }
    }
}