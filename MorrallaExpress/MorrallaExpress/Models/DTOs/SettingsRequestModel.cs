using System;

namespace MorrallaExpress.Models.DTOs
{
    public class SettingsRequestModel
    {
        public SettingsModel[] Settings { get; set; }
        public PricingModel[] Pricings { get; set; }
    }
}
