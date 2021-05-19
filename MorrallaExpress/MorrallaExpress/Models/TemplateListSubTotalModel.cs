using System;
using System.Collections.Generic;
using System.Text;
using Prism.Commands;

namespace MorrallaExpress.Models
{
    public class TemplateListSubTotalModel
    {
        public string Date { get; set; }
        public string SubTotal { get; set; }
        public DelegateCommand<TemplateListSubTotalModel> Tapped { get; set; }
            = new DelegateCommand<TemplateListSubTotalModel>((a) => { });
    }
}
