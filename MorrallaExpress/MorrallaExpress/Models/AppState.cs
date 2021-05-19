using MorrallaExpress.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Models
{
    public class AppState
    {
        #region Session
        public UserSession CurrentUser { get; set; }
        public string SessionToken { get; set; }
        public int CurrentUserStatus { get; set; }
        public bool DriverService { get; set; }
        
        #endregion

        #region Collections
        public StateModel[] AvailableStates { get; set; } = new StateModel[] { };
        public IEnumerable<UsoCFDIModel> AvailableCFDIs { get; set; } = new UsoCFDIModel[] { };
        public GeneralSettingsModel GeneralSettings { get; set; } = new GeneralSettingsModel { };
        #endregion

        #region Orders
        public IEnumerable<OrderModel> OrdersHistory { get; set; } = new OrderModel[] {};
        public IEnumerable<OrderModel> PendingOrders { get; set; } = new OrderModel[] {};
        public IEnumerable<OrderModel> ActiveOrders { get; set; } = new OrderModel[] {};
        #endregion
    }
}
