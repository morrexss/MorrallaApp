using MorrallaExpress.Models;
using MorrallaExpress.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MorrallaExpress.Helpers.Interfaces
{
    public interface IHttpService
    {
        #region Login
        #region PublicGetters
        Task<IEnumerable<StateModel>> GetAvailableStates();
        Task<IEnumerable<SubdivisionModel>> GetSubdivisions(int cityId, string query);
        Task<IEnumerable<UsoCFDIModel>> GetCFDIs();
        Task<IEnumerable<AuthorizedPersonnelModel>> GetAuthorizedPersonnelByLocation(int locationId);
        #endregion
        Task<bool> RegisterClient(RegisterClientRequestModel model);
        Task<bool> RegisterFranq(RegisterFranqRequestModel model);
        Task<bool> Login(LoginRequestModel model);
        Task<bool> CheckEmail(string email);
        void Logout();
        Task<bool> ForgotPasswordService(string email);
        Task<bool> ResetPassword(string email, string oldPw, string newPw);

        #endregion

        #region Account
        Task<IEnumerable<LocationModel>> GetLocations(bool force = false);
        Task<bool> CreateOrUpdateLocation(LocationModel model, bool create = true);
        Task<bool> DeleteLocation(int locationId);
        Task<IEnumerable<PaymentMethodModel>> GetPaymentMethods(bool force = false);
        Task<bool> CreatePaymentMethod(PaymentMethodModel model);
        Task<bool> DeletePaymentMethod(int paymentMethodId);
        Task<bool> UpdateUserData(UserSession user);
        #endregion

        #region Orders
        Task<OrderModel> GetOrder(int deliveryId);
        Task<bool> CreateOrEditOrder(OrderModelDTO model, bool create = true);
        Task<IEnumerable<OrderModel>> GetPendingOrders(bool force = false);
        Task<IEnumerable<OrderModel>> GetActiveOrders(bool force = false);
        Task<IEnumerable<OrderModel>> GetOrdersHistory(bool force = false);
        //Task<IEnumerable<OrderModel>> GetOrdersHistoryDriver(bool force = false);
        // Task<IEnumerable<OrderModel>> GetActiveOrders();
        // Task<IEnumerable<OrderModel>> GetPendingOrders();
        Task<bool> GetGeneralSettings();
        Task<bool> ReviewOrder(int rating, string reason, string review, int orderId);
        #endregion

        #region Driver
        Task<IEnumerable<OrderModel>> GetOrdersDriverAvailable(bool force = false);
        Task<IEnumerable<OrderModel>> GetOrdersDriverActive(bool force = false);
        Task<IEnumerable<OrderModel>> GetOrdersDriverHistory(bool force = false);
        Task<bool> StartDriverService(StartDriverServiceRequestModel model);
        Task<bool> StopDriverService(StartDriverServiceRequestModel model);
        Task<bool> CancelOrder(int deliveryId, double latitude, double longitude,
            int reasonId, string reason = null);
        Task<bool> DeliverOrder(int deliveryId, double latitude, double longitude);
        Task<bool> CancelOrderClient(int deliveryId, double latitude, double longitude,
            int reasonId, string reason = null);
        Task<bool> AcceptOrder(int deliveryId, double currentLat, double currentLong);
        Task<bool> UpdateLocation(int deliveryId, double latitude, double longitude);
        #endregion

        #region State
        UserSession GetCurrentUser();
        LocationModel[] GetCurrentLocations();
        PaymentMethodModel[] GetCurrentPaymentMethods();
        StateModel[] GetCurrentStates();
        DenominationModel[] GetCurrentDenominations();
        SettingsModel[] GetCurrentSettings();
        PricingModel[] GetCurrentPricings();
        OrderModel[] GetCurrentActiveOrders();
        OrderModel[] GetCurrentPendingOrders();
        OrderModel[] GetCurrentOrderHistory();
        bool SetDriverService(bool active = false);
        bool GetDriverService();
        #endregion
    }
}
