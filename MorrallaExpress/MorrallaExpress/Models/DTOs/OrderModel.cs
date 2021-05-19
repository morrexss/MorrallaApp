using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Models.DTOs
{
    public class Payout
    {
        public DateTime Date { get; set; }
    }

    public class OrderModel
    {
        public Payout Payout { get; set; }
        public int DeliveryId { get; set; }
        public string ApplicationUserId { get; set; }
        public string DriverId { get; set; }
        public int LocationId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string Status { get; set; }
        public OrderDetailModel[] Details { get; set; }
        public decimal MorrexCommision { get; set; }
        public decimal DriverComission { get; set; }
        public decimal PaypalCommision { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalExchange { get; set; }
        public decimal TotalComission { get; set; }
        public UserSession User { get; set; }
        public UserSession Driver { get; set; }
        public ReviewModel Review { get; set; }
        public LocationModel Location { get; set; }
        public string EstimatedDelivery { get; set; }
        public decimal LastKnownLat { get; set; }
        public double EstimatedDistanceInMeters { get; set; }
        public decimal LastKnownLong { get; set; }
        public bool Checked { get; set; }
        public bool Paid { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public int PaymentMethodId { get; set; }
        public bool DateModal { get; set; }
        //public string Date { get => AcceptedDate.ToString("MM/dd/yyyy "); }
    }

    public class OrderDetailModel
    {
        public int DenominationId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Value { get; set; }
    }

    public class OrderModelDTO
    {
        public string ApplicationUserId { get; set; }
        public int LocationId { get; set; }
        public string CardId { get; set; }
        public OrderDetailModelDTO[] Details { get; set; }
    }

    public class OrderDetailModelDTO
    {
        public int DenominationId { get; set; }
        public int Quantity { get; set; }
    }
}
