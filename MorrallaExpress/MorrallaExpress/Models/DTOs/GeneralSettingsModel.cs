using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Models.DTOs
{
    public class GeneralSettingsModel
    {
        public DenominationModel[] Denominations { get; set; }
        public SettingsModel[] Settings { get; set; }
        public PricingModel[] Pricing { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
