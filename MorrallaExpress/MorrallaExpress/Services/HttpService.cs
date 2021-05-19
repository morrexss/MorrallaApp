using Microsoft.AppCenter.Crashes;
using MorrallaExpress.Extensions;
using MorrallaExpress.Helpers;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using MorrallaExpress.Models.DTOs;
using Plugin.FirebasePushNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Essentials;

namespace MorrallaExpress.Services
{
    public class HttpService : IHttpService
    {
        public HttpClient _client = new HttpClient();
        public AppState CurrentState { get; } = new AppState();
        public static string _errorString;
        public HttpService() => InitState();


        #region Public
        public async Task<IEnumerable<StateModel>> GetAvailableStates()
        {
            try
            {
                if (CurrentState.AvailableStates != null && CurrentState.AvailableStates.Any())
                    return CurrentState.AvailableStates;
                var response = await _client.GetAsync(Constants.GetAvailableStatesRoute);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    CurrentState.AvailableStates = content.ToObject<StateModel[]>();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return CurrentState.AvailableStates;
        }

        public async Task<IEnumerable<UsoCFDIModel>> GetCFDIs()
        {
            try
            {
                if (CurrentState.AvailableCFDIs.Any())
                    return CurrentState.AvailableCFDIs;
                var response = await _client.GetAsync(Constants.GetAvailableCFDIsRoute);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    CurrentState.AvailableCFDIs = content.ToObject<IEnumerable<UsoCFDIModel>>();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return CurrentState.AvailableCFDIs;
        }

        public async Task<IEnumerable<SubdivisionModel>> GetSubdivisions(int cityId, string query)
        {
            SubdivisionModel[] ret = null;
            try
            {
                var queryParams = HttpUtility.ParseQueryString(string.Empty);
                queryParams["cityId"] = cityId.ToString();
                if (!string.IsNullOrEmpty(query))
                    queryParams["query"] = query;
                var response = await _client.GetAsync($"{Constants.GetAvailableSubdivisionsByCityRoute}?{queryParams.ToString()}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ret = content.ToObject<IEnumerable<SubdivisionModel>>().ToArray();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return ret;
        }

        public async Task<bool> GetGeneralSettings()
        {
            bool result = false;

            try
            {
                if (!(CurrentState?.GeneralSettings?.LastUpdate is null))
                    if (DateTime.UtcNow.Subtract(CurrentState.GeneralSettings.LastUpdate).Days < 1)
                        return true;

                var response = await _client.GetAsync(Constants.GetGeneralSettingsRoute);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var model = content.ToObject<GeneralSettingsModel>();
                    model.LastUpdate = DateTime.UtcNow;
                    CurrentState.GeneralSettings = model;
                    result = StoreSession();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            return result;
        }

        #endregion

        #region Register
        public async Task<bool> RegisterClient(RegisterClientRequestModel model)
        {
            var result = false;
            try
            {
                model.FireBaseToken = CrossFirebasePushNotification.Current.Token;
                var reqContent = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(Constants.RegisterClientRoute, reqContent);
                result = response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created;
                if (result)
                    return await Login(new LoginRequestModel { Email = model.Email, Password = model.Password });
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        public async Task<bool> RegisterFranq(RegisterFranqRequestModel model)
        {
            var result = false;
            try
            {
                var reqContent = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(Constants.RegisterFranqRoute, reqContent);
                result = response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }
        #endregion

        #region Login
        public async Task<bool> Login(LoginRequestModel model)
        {
            var result = false;
            try
            {
                var reqContent = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(Constants.LoginRoute, reqContent);
                result = response.StatusCode == HttpStatusCode.OK;
                if (result)
                {
                    var responseStr = await response.Content.ReadAsStringAsync();
                    var responseContent = responseStr.ToObject<LoginResponseModel>();
                    CurrentState.CurrentUser = responseContent.UserData;
                    CurrentState.SessionToken = responseContent.Token;
                    CurrentState.CurrentUserStatus = responseContent.Status;
                    result = StoreSession();

                    var getExtraInfo = await _client.GetAsync($"{Constants.GetExtraInfouser + responseContent.UserData.UserId}");
                    var extraInfoStr = await getExtraInfo.Content.ReadAsStringAsync();
                    var extraInfoContent = extraInfoStr.ToObject<UserSession>();

                    CurrentState.CurrentUser.Rating = extraInfoContent?.Rating;
                    CurrentState.CurrentUser.BankName = extraInfoContent?.BankName;
                    CurrentState.CurrentUser.BankAccountHolderName = extraInfoContent?.BankAccountHolderName;
                    CurrentState.CurrentUser.BankAccountClabe = extraInfoContent?.BankAccountClabe;
                    result = StoreSession();
                }
                else
                    _errorString = "Inténtelo de nuevo. \n Usuario/ Contraseña inválidos.";
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        public async Task<bool> CheckEmail(string email)
        {
            bool ret = false;
            try
            {
                var queryParams = HttpUtility.ParseQueryString(string.Empty);
                queryParams["email"] = email;
                var response = await _client.GetAsync($"{Constants.CheckEmailRoute}?{queryParams.ToString()}");
                var resp = (await response.Content.ReadAsStringAsync()).ToObject<CheckEmailResponseModel>();
                ret = resp.Exists != "true";
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return ret;
        }


        public void Logout()
        {
            ResetState();
            DeleteSession();
        }



        public async Task<bool> ForgotPasswordService(string email)
        {
            var result = false;
            try
            {
                var model = new { email };
                var reqContent = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(Constants.ForgotPasswordRoute, reqContent);
                result = response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }
        #endregion

        #region Account
        #region Profile
        public async Task<bool> UpdateUserData(UserSession model)
        {
            bool result = false;
            try
            {
                var reqContent = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await _client.PutAsync($"{Constants.UpdateUserRoute}/{model.UserId}", reqContent);
                var wea = (await response.Content.ReadAsStringAsync()).ToObject<UserSession>();
                result = response.StatusCode == HttpStatusCode.OK;
                if (result)
                {
                    model.Photo = wea.Photo;
                    CurrentState.CurrentUser = model;
                    StoreSession();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        public async Task<bool> ResetPassword(string email, string oldPw, string newPw)
        {
            bool result = false;
            try
            {
                var reqCodeModel = new { email };
                var reqContent = new StringContent(reqCodeModel.ToJson(), Encoding.UTF8, "application/json");
                var codeResponse = await _client.PostAsync(Constants.ForgotPasswordRoute, reqContent);
                result = codeResponse.StatusCode == HttpStatusCode.OK;
                if (result)
                {
                    var codeResp = (await codeResponse.Content.ReadAsStringAsync()).ToObject<CodeResponseModel>();
                    var reqChangeModel = new { email, code = codeResp.Code, password = newPw };
                    reqContent = new StringContent(reqChangeModel.ToJson(), Encoding.UTF8, "application/json");
                    var changeResponse = await _client.PostAsync(Constants.ResetPasswordRoute, reqContent);
                    result = changeResponse.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }
        #endregion
        #region PaymentMethods
        public async Task<IEnumerable<PaymentMethodModel>> GetPaymentMethods(bool force = false)
        {
            try
            {
                if (!force)
                    return CurrentState.CurrentUser.PaymentMethods;
                var response = await _client.GetAsync(Constants.PaymentMethodsRoute);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    CurrentState.CurrentUser.PaymentMethods = content.ToObject<PaymentMethodModel[]>();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return CurrentState.CurrentUser.PaymentMethods;
        }

        public async Task<bool> CreatePaymentMethod(PaymentMethodModel model)
        {
            var result = false;
            try
            {
                var reqContent = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(Constants.PaymentMethodsRoute, reqContent);
                result = response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created;
                if (result)
                {
                    var newPayment = (await response.Content.ReadAsStringAsync()).ToObject<PaymentMethodModel>();
                    var paymentMethods = CurrentState.CurrentUser.PaymentMethods.ToList();
                    paymentMethods.Add(newPayment);
                    CurrentState.CurrentUser.PaymentMethods = paymentMethods.ToArray();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        public async Task<bool> DeletePaymentMethod(int paymentMethodId)
        {
            var result = false;
            try
            {
                var response = await _client.DeleteAsync($"{Constants.PaymentMethodsRoute}/{paymentMethodId.ToString()}");
                result = response.StatusCode == HttpStatusCode.OK;
                if (result)
                {
                    var paymentMethods = CurrentState.CurrentUser.PaymentMethods.ToList();
                    paymentMethods.Remove(paymentMethods.FirstOrDefault(loc => loc.PaymentMethodId == paymentMethodId));
                    CurrentState.CurrentUser.PaymentMethods = paymentMethods.ToArray();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }
        #endregion
        #region Locations
        public async Task<IEnumerable<AuthorizedPersonnelModel>> GetAuthorizedPersonnelByLocation(int locationId)
        {
            try
            {
                var response = await _client.GetAsync(Constants.GetAuthorizedPersonnelRoute);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var toAdd = (await response.Content.ReadAsStringAsync()).ToObject<IEnumerable<AuthorizedPersonnelModel>>();
                    var location = GetCurrentLocations().FirstOrDefault(loc => loc.LocationId == locationId);
                    var authPersonnel = location.AuthorizedPersonnel.ToList();
                    authPersonnel.AddRange(toAdd);
                    location.AuthorizedPersonnel = authPersonnel.ToArray();
                    return location.AuthorizedPersonnel;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return null;
        }

        public async Task<IEnumerable<LocationModel>> GetLocations(bool force = false)
        {
            try
            {
                if (!force)
                    return CurrentState.CurrentUser.Locations;
                var response = await _client.GetAsync(Constants.LocationsRoute);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    CurrentState.CurrentUser.Locations = content.ToObject<LocationModel[]>();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return CurrentState.CurrentUser.Locations;
        }

        public async Task<bool> CreateOrUpdateLocation(LocationModel model, bool create = true)
        {
            var result = false;
            try
            {
                var reqContent = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                if (create)
                    response = await _client.PostAsync(Constants.LocationsRoute, reqContent);
                else
                    response = await _client.PutAsync($"{Constants.LocationsRoute}/{model.LocationId}", reqContent);
                result = response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created;
                if (result)
                {
                    var newLoc = (await response.Content.ReadAsStringAsync()).ToObject<LocationModel>();
                    var locations = CurrentState.CurrentUser.Locations.ToList();
                    locations.Add(newLoc);
                    CurrentState.CurrentUser.Locations = locations.ToArray();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        public async Task<bool> DeleteLocation(int locationId)
        {
            var result = false;
            try
            {
                var response = await _client.DeleteAsync($"{Constants.LocationsRoute}/{locationId}");
                result = response.StatusCode == HttpStatusCode.OK;
                if (result)
                {
                    var locations = CurrentState.CurrentUser.Locations.ToList();
                    locations.Remove(locations.FirstOrDefault(loc => loc.LocationId == locationId));
                    CurrentState.CurrentUser.Locations = locations.ToArray();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }
        #endregion
        #endregion

        #region Driver
        // pending = new, active = accepted
        public async Task<bool> StartDriverService(StartDriverServiceRequestModel model)
        {
            var result = false;
            try
            {
                var reqContent = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(Constants.StartDriverServiceRoute, reqContent);
                result = response.StatusCode == HttpStatusCode.OK;
                SetDriverService(result);
                StoreSession();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        public async Task<bool> StopDriverService(StartDriverServiceRequestModel model)
        {
            var result = false;
            try
            {
                var reqContent = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(Constants.StopDriverServiceRoute, reqContent);
                result = response.StatusCode == HttpStatusCode.OK;
                SetDriverService(!result);
                StoreSession();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        public async Task<bool> AcceptOrder(int deliveryId, double currentLat, double currentLong)
        {
            OrderModel orderUpdated = await UpdateOrder(deliveryId, GetCurrentUser().UserId, currentLat, currentLong, Constants.AcceptedOrderStatus);
            if (orderUpdated == null)
                return false;

            var pendings = GetCurrentPendingOrders().ToList();
            var toRemove = pendings.FirstOrDefault(o => o.DeliveryId == deliveryId);
            if (pendings.Remove(toRemove))
                CurrentState.PendingOrders = pendings;

            var actives = CurrentState.ActiveOrders.ToList();
            actives.Insert(0, orderUpdated);
            CurrentState.ActiveOrders = actives;

            return true;
        }

        public async Task<bool> CancelOrder(int deliveryId, double currentLat, double currentLong, int reasonId, string reason = null)
        {
            OrderModel orderUpdated = await UpdateOrder(deliveryId, GetCurrentUser().UserId, currentLat, currentLong,
                Constants.CanceledByDriverOrderStatus, reasonId, message: reason);
            if (orderUpdated == null)
                return false;

            var actives = CurrentState.ActiveOrders.ToList();
            var toRemove = actives.FirstOrDefault(o => o.DeliveryId == deliveryId);
            if (actives.Remove(toRemove))
                CurrentState.ActiveOrders = actives;

            var history = CurrentState.OrdersHistory.ToList();
            history.Add(orderUpdated);
            CurrentState.OrdersHistory = history;

            return true;
        }

        public async Task<bool> DeliverOrder(int deliveryId, double currentLat, double currentLong)
        {
            OrderModel orderUpdated = await UpdateOrder(deliveryId, GetCurrentUser().UserId, currentLat, currentLong, Constants.DeliveredOrderStatus);
            if (orderUpdated == null)
                return false;

            var actives = CurrentState.ActiveOrders.ToList();
            var toRemove = actives.FirstOrDefault(o => o.DeliveryId == deliveryId);
            if (actives.Remove(toRemove))
                CurrentState.ActiveOrders = actives;

            var history = CurrentState.OrdersHistory.ToList();
            history.Add(orderUpdated);
            CurrentState.OrdersHistory = history;

            return true;
        }

        public async Task<bool> UpdateLocation(int deliveryId, double latitude, double longitude)
        {
            var result = false;
            try
            {
                var model = new { deliveryId, userId = GetCurrentUser().UserId, latitude, longitude, note = "Location Update" };
                var reqContent = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(Constants.LogsRoute, reqContent);
                result = response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created;
                // actualizar state con deliverId a esos lat y long
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        public async Task<IEnumerable<OrderModel>> GetOrdersDriverAvailable(bool force = false)
        {
            try
            {
                if (!force)
                    return CurrentState.PendingOrders;
                var orders = await GetOrdersDriver("New");
                if (orders != null)
                    CurrentState.PendingOrders = orders;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return CurrentState.PendingOrders;
        }

        public async Task<IEnumerable<OrderModel>> GetOrdersDriverActive(bool force = false)
        {
            try
            {
                if (!force)
                    return CurrentState.ActiveOrders;
                var orders = await GetOrdersDriver("Accepted");
                if (orders != null)
                    CurrentState.ActiveOrders = orders;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return CurrentState.ActiveOrders;
        }

        public async Task<IEnumerable<OrderModel>> GetOrdersDriverHistory(bool force = false)
        {
            try
            {
                if (!force)
                    return CurrentState.OrdersHistory;
                var orders = await GetOrdersDriver();
                if (orders != null)
                {
                    CurrentState.OrdersHistory = orders.Where(order => order.Status != Constants.NewOrderStatus && order.Status != Constants.AcceptedOrderStatus);
                    CurrentState.PendingOrders = orders.Where(order => order.Status == Constants.NewOrderStatus);
                    CurrentState.ActiveOrders = orders.Where(order => order.Status == Constants.AcceptedOrderStatus);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return CurrentState.OrdersHistory;
        }

        internal async Task<IEnumerable<OrderModel>> GetOrdersDriver(string status = "")
        {
            IEnumerable<OrderModel> result = new OrderModel[] { };
            try
            {
                string statusFiltro = string.IsNullOrEmpty(status) ? "" : $"?status={status}";
                var response = await _client.GetAsync($"{Constants.OrdersRoute}/driver/{GetCurrentUser().UserId}{statusFiltro}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (content.Count() == 0)
                        return new OrderModel[] { };
                    result = content.ToObject<IEnumerable<OrderModel>>();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        #endregion

        #region Orders
        public async Task<OrderModel> GetOrder(int deliveryId)
        {
            OrderModel ret = null;
            try
            {
                var response = await _client.GetAsync($"{Constants.OrdersRoute}/{deliveryId}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ret = content.ToObject<OrderModel>();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return ret;
        }

        public async Task<bool> CreateOrEditOrder(OrderModelDTO model, bool create = true)
        {
            var result = false;
            try
            {
                var reqContent = new StringContent(model.ToJson(), Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                if (create)
                    response = await _client.PostAsync(Constants.OrdersRoute, reqContent);
                else
                    response = await _client.PutAsync($"{Constants.OrdersRoute}", reqContent);
                result = response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created;
                if (result)
                {
                    var newOrder = (await response.Content.ReadAsStringAsync()).ToObject<OrderModel>();
                    if (create)
                    {
                        var list = CurrentState.PendingOrders.ToList();
                        list.Add(newOrder);
                        CurrentState.PendingOrders = list;
                    }
                    else
                    {
                        var list = CurrentState.ActiveOrders.ToList();
                        list.Add(newOrder);
                        CurrentState.ActiveOrders = list;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        public async Task<bool> CancelOrderClient(int deliveryId, double currentLat, double currentLong, int reasonId, string reason = null)
        {
            OrderModel orderUpdated = await UpdateOrder(deliveryId, GetCurrentUser().UserId, currentLat, currentLong,
                Constants.CanceledByUserdOrderStatus, reasonId: reasonId, message: reason);
            if (orderUpdated == null)
                return false;

            var pendings = CurrentState.PendingOrders.ToList();
            var toRemovepending = pendings.FirstOrDefault(o => o.DeliveryId == deliveryId);
            if (pendings.Remove(toRemovepending))
                CurrentState.PendingOrders = pendings;

            var actives = CurrentState.ActiveOrders.ToList();
            var toRemoveactive = actives.FirstOrDefault(o => o.DeliveryId == deliveryId);
            if (actives.Remove(toRemoveactive))
                CurrentState.ActiveOrders = actives;

            var history = CurrentState.OrdersHistory.ToList();
            history.Add(orderUpdated);
            CurrentState.OrdersHistory = history;

            return true;
        }

        internal async Task<OrderModel> UpdateOrder(int deliveryId, string driverId, double currentLat, double currentLong, string action,
            int? reasonId = null, string message = null)
        {
            OrderModel result = null;
            try
            {
                var model = new { deliveryId, driverId, currentLat, currentLong, action, reasonId, message };
                var json = model.ToJson();
                var reqContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PutAsync($"{Constants.OrdersRoute}/{deliveryId}", reqContent);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    var orderUpdated = await response.Content.ReadAsStringAsync();
                    result = orderUpdated.ToObject<OrderModel>();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        public async Task<IEnumerable<OrderModel>> GetPendingOrders(bool force = false)
        {
            try
            {
                if (!force)
                    return CurrentState.PendingOrders;
                var orders = await GetOrdersClient("New");
                if (orders != null)
                    CurrentState.PendingOrders = orders;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return CurrentState.PendingOrders;
        }

        public async Task<IEnumerable<OrderModel>> GetActiveOrders(bool force = false)
        {
            try
            {
                if (!force)
                    return CurrentState.ActiveOrders;
                var orders = await GetOrdersClient("Accepted");
                if (orders != null)
                    CurrentState.ActiveOrders = orders;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return CurrentState.ActiveOrders;
        }

        public async Task<IEnumerable<OrderModel>> GetOrdersHistory(bool force = false)
        {
            try
            {
                if (!force)
                    return CurrentState.OrdersHistory;
                var orders = await GetOrdersClient();
                if (orders != null)
                {
                    CurrentState.PendingOrders = orders.Where(order => order.Status == Constants.NewOrderStatus).ToArray();
                    CurrentState.ActiveOrders = orders.Where(order => order.Status == Constants.AcceptedOrderStatus).ToArray();
                    CurrentState.OrdersHistory = orders.Where(order => order.Status != Constants.NewOrderStatus && order.Status != Constants.AcceptedOrderStatus).ToArray();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return CurrentState.OrdersHistory;
        }

        internal async Task<IEnumerable<OrderModel>> GetOrdersClient(string status = "")
        {
            IEnumerable<OrderModel> result = new OrderModel[] { };
            try
            {
                string statusFiltro = string.IsNullOrEmpty(status) ? "" : $"?status={status}";
                var userId = GetCurrentUser().UserId;
                var response = await _client.GetAsync($"{Constants.OrdersRoute}/user/{GetCurrentUser().UserId}{statusFiltro}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    result = content.ToObject<IEnumerable<OrderModel>>();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        public async Task<IEnumerable<LocationModel>> GetLocations2(bool force = false)
        {
            try
            {
                if (!force)
                    return CurrentState.CurrentUser.Locations;
                var response = await _client.GetAsync(Constants.LocationsRoute);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    CurrentState.CurrentUser.Locations = content.ToObject<LocationModel[]>();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return CurrentState.CurrentUser.Locations;
        }

        public async Task<bool> ReviewOrder(int rating, string reason, string review, int orderId)
        {
            bool result = false;
            try
            {
                var reqModel = new { score = rating, reviewCategoryId = 1, message = review, reviewId = orderId };
                var reqContent = new StringContent(reqModel.ToJson(), Encoding.UTF8, "application/json");
                var codeResponse = await _client.PostAsync(Constants.ReviewOrderRoute, reqContent);
                result = codeResponse.StatusCode == HttpStatusCode.OK || codeResponse.StatusCode == HttpStatusCode.Created;
                if (result)
                {
                    var reviewJson = await codeResponse.Content.ReadAsStringAsync();
                    var reviewModel = reviewJson.ToObject<ReviewModel>();
                    var orders = GetCurrentOrderHistory().ToList();
                    var order = orders.FirstOrDefault(ord => ord.DeliveryId == orderId);
                    order.Review = reviewModel;
                    CurrentState.OrdersHistory = orders.ToArray();
                }

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }
        #endregion

        #region State
        public UserSession GetCurrentUser() =>
            CurrentState.CurrentUser;
        public LocationModel[] GetCurrentLocations() =>
            CurrentState.CurrentUser.Locations;
        public PaymentMethodModel[] GetCurrentPaymentMethods() =>
            CurrentState.CurrentUser.PaymentMethods;
        public StateModel[] GetCurrentStates() =>
            CurrentState?.AvailableStates?.ToArray();
        public OrderModel[] GetCurrentActiveOrders() =>
            CurrentState?.ActiveOrders?.ToArray();
        public OrderModel[] GetCurrentPendingOrders() =>
           CurrentState?.PendingOrders?.ToArray();
        public OrderModel[] GetCurrentOrderHistory() =>
           CurrentState?.OrdersHistory?.ToArray();
        public DenominationModel[] GetCurrentDenominations() =>
           CurrentState?.GeneralSettings?.Denominations;
        public SettingsModel[] GetCurrentSettings() =>
           CurrentState?.GeneralSettings?.Settings;
        public PricingModel[] GetCurrentPricings() =>
           CurrentState?.GeneralSettings?.Pricing;
        public bool SetDriverService(bool active = false) =>
            CurrentState.DriverService = active;
        public bool GetDriverService() =>
            CurrentState.DriverService;

        internal void ResetState()
        {
            CurrentState.CurrentUser = null;
            CurrentState.SessionToken = null;
            CurrentState.CurrentUserStatus = 0;
            CurrentState.DriverService = false;

            CurrentState.OrdersHistory = new OrderModel[] { };
            CurrentState.PendingOrders = new OrderModel[] { };
            CurrentState.ActiveOrders = new OrderModel[] { };
        }

        #endregion

        #region Preferences
        bool StoreSession()
        {
            var result = DeleteSession();
            try
            {
                Preferences.Set(Constants.SessionTokenKey, CurrentState.SessionToken);
                Preferences.Set(Constants.SessionUserStatusKey, CurrentState.CurrentUserStatus);
                Preferences.Set(Constants.SessionUserDataKey, CurrentState.CurrentUser.ToJson());
                Preferences.Set(Constants.GeneralSettingsKey, CurrentState.GeneralSettings.ToJson());
                Preferences.Set(Constants.DriverServiceKey, CurrentState.DriverService);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(CurrentState.SessionToken);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        bool DeleteSession()
        {
            var result = false;
            try
            {
                if (Preferences.ContainsKey(Constants.SessionTokenKey))
                    Preferences.Remove(Constants.SessionTokenKey);
                if (Preferences.ContainsKey(Constants.SessionUserStatusKey))
                    Preferences.Remove(Constants.SessionUserStatusKey);
                if (Preferences.ContainsKey(Constants.SessionUserDataKey))
                    Preferences.Remove(Constants.SessionUserDataKey);
                if (Preferences.ContainsKey(Constants.GeneralSettingsKey))
                    Preferences.Remove(Constants.GeneralSettingsKey);
                if (Preferences.ContainsKey(Constants.DriverServiceKey))
                    Preferences.Remove(Constants.DriverServiceKey);
                result = true;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            return result;
        }

        void InitState()
        {
            try
            {
                if (Preferences.ContainsKey(Constants.SessionTokenKey))
                    CurrentState.SessionToken = Preferences.Get(Constants.SessionTokenKey, null);
                if (Preferences.ContainsKey(Constants.SessionUserStatusKey))
                    CurrentState.CurrentUserStatus = Preferences.Get(Constants.SessionUserStatusKey, 0);
                if (Preferences.ContainsKey(Constants.SessionUserDataKey))
                {
                    var userJson = Preferences.Get(Constants.SessionUserDataKey, null);
                    CurrentState.CurrentUser = userJson.ToObject<UserSession>();
                }
                if (Preferences.ContainsKey(Constants.GeneralSettingsKey))
                {
                    var settingsJson = Preferences.Get(Constants.GeneralSettingsKey, null);
                    CurrentState.GeneralSettings = settingsJson.ToObject<GeneralSettingsModel>();
                }
                if (Preferences.ContainsKey(Constants.DriverServiceKey))
                    CurrentState.DriverService = Preferences.Get(Constants.DriverServiceKey, false);
                if (!string.IsNullOrEmpty(CurrentState.SessionToken))
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(CurrentState.SessionToken);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }
        #endregion
    }
}
