using System;

namespace MorrallaExpress.Models
{
    public class DenominationItem
    {
        public int DenominationId { get; set; }
        public int Quantity { get; set; }
        public decimal Value { get; set; }
        public decimal Total { get => Value * Quantity; }
    }

    public class DenominationPickers
    {
        public DenominationItem[] One { get; set; } = new DenominationItem[]{ };
        public DenominationItem[] Two { get; set; } = new DenominationItem[]{ };
        public DenominationItem[] Five { get; set; } = new DenominationItem[]{ };
        public DenominationItem[] Ten { get; set; } = new DenominationItem[]{ };
        public DenominationItem[] Twenty { get; set; } = new DenominationItem[]{ };
    }
}
