using System;

namespace MorrallaExpress.Models.DTOs
{
    public class PricingModel
    {
        public int PriceChartId { get; set; }
        public int MinRange { get; set; }
        public int MaxRange { get; set; }
        public double MorrexComision { get; set; }
        public double DriverComision { get; set; }
    }
}
