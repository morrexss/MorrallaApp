using Prism;
using Prism.Ioc;
using MorrallaExpress.ViewModels;
using MorrallaExpress.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MorrallaExpress.Views.Login;
using MorrallaExpress.ViewModels.Login;
using MorrallaExpress.Views.Account;
using MorrallaExpress.ViewModels.Account;
using MorrallaExpress.Views.Ours;
using MorrallaExpress.ViewModels.Ours;
using MorrallaExpress.Views.Legal;
using MorrallaExpress.ViewModels.Legal;
using MorrallaExpress.Views.Finance;
using MorrallaExpress.ViewModels.Finance;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Services;
using MorrallaExpress.Views.Orders;
using MorrallaExpress.ViewModels.Orders;
using Xamarin.Essentials;
using MorrallaExpress.Helpers;
using Openpay.Xamarin;
using MorrallaExpress.Views.Driver;
using Prism.Services;
using MorrallaExpress.Models;
using MorrallaExpress.Extensions;
using System.Linq;
using System;
using Device = Xamarin.Forms.Device;
using System.Threading.Tasks;
using Plugin.FirebasePushNotification;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MorrallaExpress
{
    public partial class App
    {
        readonly string _iOSAppCenterKey = "ios=d7c45047-582a-4686-95ef-8c8a48f340dd;";
        readonly string _droidAppCenterKey = "android=c84691ee-7671-48a7-a58a-b7f26dafd0b0;";
        readonly string _merchantId = "mp8kw7vzjwpvy88jv6kb";
        readonly string _openPayApiKey = "sk_b8e70f6f37894b0bb70ed9a5ade57253";
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            InitBackgroundJobs();
            if (CrossOpenpay.IsSupported)
                CrossOpenpay.Current.Initialize(_merchantId, _openPayApiKey, false);

            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");
            };

            if (Preferences.ContainsKey(Constants.SessionTokenKey))
            {
                var sesion = Preferences.Get(Constants.SessionUserDataKey, null);
                if (sesion != null)
                {
                    var model = sesion.ToObject<UserSession>();
                    if (model.Roles.Contains("User"))
                        await NavigationService.NavigateAsync("/HomeMasterDetailPage/NavigationPage/OrdersTabbedPage?selectedTab=PendingOrdersPage");
                    else
                        await NavigationService.NavigateAsync("/HomeMasterDetailPage/NavigationPage/OrdersDriverTabbedPage?selectedTab=AvailableOrdersDriverPage");
                }
            }
            else
            {
                //if (Preferences.ContainsKey(Constants.WelcomeTriggeredKey))
                //    await NavigationService.NavigateAsync("/NavigationPage/LoginPage");
                //else
                //{
                    Preferences.Set(Constants.WelcomeTriggeredKey, true);
                    await NavigationService.NavigateAsync("/NavigationPage/LoginPage/WelcomePage");
                //}
            }
        }

        #region LifeCycles
        protected override void OnStart()
        {
            base.OnStart();
            AppCenter.Start(_iOSAppCenterKey + _droidAppCenterKey,
                typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            // TODO: This is the time to save app data in case the process is terminated.
            // This is the perfect timing to release exclusive resources (camera, I/O devices, etc...)
        }

        protected override void OnResume()
        {
            base.OnResume();
            // TODO: Refresh network data, perform UI updates, and reacquire resources like cameras, I/O devices, etc.
        }
        #endregion

        void InitBackgroundJobs()
        {
            var httpService = Container.Resolve<IHttpService>();
            var settings = httpService.GetCurrentSettings();
            var driverSetting = settings?.FirstOrDefault(s => s.SettingId == 13);
            var timeLapseMinutes = 5;

            if (driverSetting != null)
                timeLapseMinutes = Convert.ToInt32(driverSetting.Value);
            Device.StartTimer(new TimeSpan(0, timeLapseMinutes, 0), () =>
            {
                settings = httpService.GetCurrentSettings();
                if (settings == null)
                    httpService.GetGeneralSettings();

                System.Diagnostics.Debug.WriteLine(string.Format("Driver: Update location."));
                var usr = httpService.GetCurrentUser();
                if (usr?.Roles.Contains("Driver") ?? false)
                {
                    var orders = httpService.GetCurrentActiveOrders();
                    var pos = Geolocation.GetLastKnownLocationAsync().Result;
                    foreach (var order in orders)
                        httpService.UpdateLocation(order.DeliveryId, pos.Latitude, pos.Longitude);
                }
                return true;
            });
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Services
            containerRegistry.RegisterSingleton<IHttpService, HttpService>();

            // Utilities
            containerRegistry.RegisterForNavigation<NavigationPage, NavigationPageViewModel>();
            containerRegistry.RegisterForNavigation<HomeMasterDetailPage, HomeMasterDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<AccountTabbedPage>();
            containerRegistry.RegisterForNavigation<OrdersTabbedPage>();

            // Login
            containerRegistry.RegisterForNavigation<WelcomePage, WelcomePageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterFranqPage, RegisterFranqPageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterClientPage, RegisterClientPageViewModel>();
            containerRegistry.RegisterForNavigation<ForgotPassword, ForgotPasswordViewModel>();

            // Account
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<AccountPage, AccountPageViewModel>();
            containerRegistry.RegisterForNavigation<EditProfilePage, EditProfilePageViewModel>();
            containerRegistry.RegisterForNavigation<ChangePasswordPage, ChangePasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<LocationsPage, LocationsPageViewModel>();
            containerRegistry.RegisterForNavigation<LocationDetailPage, LocationDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<PaymentsPage, PaymentsPageViewModel>();
            containerRegistry.RegisterForNavigation<PaymentsDetailPage, PaymentsDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<FiscalPage, FiscalPageViewModel>();
            containerRegistry.RegisterForNavigation<EditFiscalPage, EditFiscalPageViewModel>();

            // Orders
            containerRegistry.RegisterForNavigation<PendingOrdersPage, PendingOrdersPageViewModel>();
            containerRegistry.RegisterForNavigation<OrdersHistoryPage, OrdersHistoryPageViewModel>();
            containerRegistry.RegisterForNavigation<ActiveOrdersPage, ActiveOrdersPageViewModel>();
            containerRegistry.RegisterForNavigation<OrderDetailPage, OrderDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<AccountDriverTabbedPage, AccountDriverTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<OrdersDriverTabbedPage, OrdersDriverTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<ProfileDriverPage, ProfileDriverPageViewModel>();
            containerRegistry.RegisterForNavigation<BillingDriverPage, BillingDriverPageViewModel>();
            containerRegistry.RegisterForNavigation<AvailableOrdersDriverPage, AvailableOrdersDriverPageViewModel>();
            containerRegistry.RegisterForNavigation<ActiveOrdersDriverPage, ActiveOrdersDriverPageViewModel>();
            containerRegistry.RegisterForNavigation<HistoryOrdersDriverPage, HistoryOrdersDriverPageViewModel>();
            containerRegistry.RegisterForNavigation<OrderDetailDriverPage, OrderDetailDriverPageViewModel>();
            containerRegistry.RegisterForNavigation<NewOrderPage, NewOrderPageViewModel>();
            containerRegistry.RegisterForNavigation<DriverDetailPage, DriverDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<ReviewDelivery, ReviewDeliveryViewModel>();
            containerRegistry.RegisterForNavigation<LegalPage, LegalPageViewModel>();
            containerRegistry.RegisterForNavigation<DenominationsPage, DenominationsPageViewModel>();
            containerRegistry.RegisterForNavigation<CancelOrderClient, CancelOrderClientViewModel>();
            containerRegistry.RegisterForNavigation<CancelOrderDriver, CancelOrderDriverViewModel>();

            // Ours
            containerRegistry.RegisterForNavigation<OursTabbedPage, OursTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<ContactUsPage, ContactUsPageViewModel>();
            containerRegistry.RegisterForNavigation<LogisticPage, LogisticPageViewModel>();
            containerRegistry.RegisterForNavigation<OursPage, OursPageViewModel>();
            containerRegistry.RegisterForNavigation<LogisticClientPage, LogisticClientPageViewModel>();


            // Legal
            containerRegistry.RegisterForNavigation<LegalTabbedPage, LegalTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<LegalPage, LegalPageViewModel>();
            containerRegistry.RegisterForNavigation<ContractPage, ContractPageViewModel>();

            // Finance
            containerRegistry.RegisterForNavigation<FinanceTabbedPage, FinanceTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<FinancePendingPage, FinancePendingPageViewModel>();
            containerRegistry.RegisterForNavigation<FinanceHistoryPage, FinanceHistoryPageViewModel>();

            containerRegistry.RegisterForNavigation<DriverScorePage, DriverScorePageViewModel>();
            containerRegistry.RegisterForNavigation<LegalDriverTabbedPage, LegalDriverTabbedPageViewModel>();
            containerRegistry.RegisterForNavigation<TermsAndConditionsDriverPage, TermsAndConditionsDriverPageViewModel>();
            containerRegistry.RegisterForNavigation<PrivacyPolicyDriverPage, PrivacyPolicyDriverPageViewModel>();
        }
    }
}
