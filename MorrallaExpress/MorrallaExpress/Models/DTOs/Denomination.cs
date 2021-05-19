using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Models.DTOs
{
    public class DenominationModel
    {
        public int DenominationId { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Weight { get; set; }
        public string WeightString { get; set; }
        public int Packaging { get; set; }
        public int MaxPackaging { get; set; }

        public DenominationItem[] Details { get; set; } = new DenominationItem[] { };
    }

    public class DenominationItem
    {
        public int DenominationId { get; set; }
        public int Quantity { get; set; }
        public int MaxPackaging { get; set; }
        public decimal Value { get; set; }
        public decimal Total { get => Quantity * Value; }
    }
}
