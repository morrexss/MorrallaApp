using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Helpers
{
    public static class Constants
    {
        public static string NewOrderStatus = "new";
        public static string AcceptedOrderStatus = "accepted";
        public static string DeliveredOrderStatus = "delivered";
        public static string CanceledByUserdOrderStatus = "cancelledbyuser";
        public static string CanceledByDriverOrderStatus = "cancelledbydriver";

        static readonly string _baseUrl = "https://morrexssapp.azurewebsites.net/api/";
        // Public Gets
        public static string GetAvailableStatesRoute = $"{_baseUrl}states";
        public static string GetAvailableSubdivisionsByCityRoute = $"{_baseUrl}subdivisions";
        public static string GetAvailableCFDIsRoute = $"{_baseUrl}usos";
        public static string GetAuthorizedPersonnelRoute = $"{_baseUrl}usos";
        public static string GetGeneralSettingsRoute = $"{_baseUrl}settings";
        public static string GetExtraInfouser = $"{_baseUrl}users/driver/";

        // User posts
        public static string RegisterClientRoute = $"{_baseUrl}users";
        public static string RegisterFranqRoute = $"{_baseUrl}franchiserequests";
        public static string LoginRoute = $"{_baseUrl}users/login";
        public static string UpdateUserRoute = $"{_baseUrl}users";
        public static string CheckEmailRoute = $"{_baseUrl}checkemail";
        public static string ForgotPasswordRoute = $"{_baseUrl}users/forgotpassword";
        public static string ResetPasswordRoute = $"{_baseUrl}users/resetpassword";

        // Locations
        public static string LocationsRoute = $"{_baseUrl}locations";

        // Payment Methods
        public static string PaymentMethodsRoute = $"{_baseUrl}paymentmethods";

        // Orders
        public static string OrdersRoute = $"{_baseUrl}deliveries";
        public static string GetOrdersHistoryRoute = $"{_baseUrl}deliveries";
        public static string GetPendingOrdersRoute = $"{_baseUrl}deliveries";
        public static string GetActiveOrdersRoute = $"{_baseUrl}deliveries";
        public static string ReviewOrderRoute = $"{_baseUrl}reviews";

        //events
        // public static string OrdersLoadedEvent = $"OrdersLoadedEvent";

        // Logs
        public static string LogsRoute = $"{_baseUrl}logs";

        //Drivers routes
        public static string StartDriverServiceRoute = $"{_baseUrl}services/start";
        public static string StopDriverServiceRoute = $"{_baseUrl}services/stop";

        // App Preferences
        public static string SessionTokenKey = "SessionToken";
        public static string SessionUserStatusKey = "SessionUserStatus";
        public static string SessionUserDataKey = "SessionUserDataToken";
        public static string DriverServiceKey = "UserDriverServiceKey";
        public static string WelcomeTriggeredKey = "WelcomeTriggered";


        public static string GeneralSettingsKey = "GeneralSettings";
        public static string TransactionSettingsKey = "TransactionSettings";

    }
}
