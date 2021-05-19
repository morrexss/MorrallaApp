using MorrallaExpress.Models;
using MorrallaExpress.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MorrallaExpress.Views.Finance
{
    public class FinanceListTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Detail { get; set; }
        public DataTemplate SubTotal { get; set; }
        public FinanceListTemplateSelector()
        {
            Detail = new DataTemplate(typeof(FinanceListDetailTemplate));
            SubTotal = new DataTemplate(typeof(FinanceListSubTotalTemplate));
        }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is OrderModel)
            {
                return Detail;
            }
            else if (item is TemplateListSubTotalModel)
            {
                return SubTotal;
            }
            return null;
        }
    }
}
