using MorrallaExpress.Models;
using MorrallaExpress.Views.Templates;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MorrallaExpress.Views
{
    public class WelcomeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Welcome1 { get; set; }
        public DataTemplate Welcome2 { get; set; }
        public DataTemplate Welcome3 { get; set; }

        public WelcomeTemplateSelector()
        {
            // Retain instances!
            Welcome1 = new DataTemplate(typeof(WelcomePageStep1));
            Welcome2 = new DataTemplate(typeof(WelcomePageStep2));
            Welcome3 = new DataTemplate(typeof(WelcomePageStep3));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var selected = item as CarouselList;
            switch (selected.Id)
            {
                case 1: return Welcome1;
                case 2: return Welcome2;
                case 3: return Welcome3;
                default: return Welcome1;
            }
        }
    }
}


