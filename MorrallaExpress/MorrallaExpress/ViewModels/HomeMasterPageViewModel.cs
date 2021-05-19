using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MorrallaExpress.ViewModels
{
    public class HomeMasterDetailPageViewModel : ViewModelBase
    {
        #region Commands
        #endregion

        #region Props
        public UserSession User { get; set; }
        public bool DriverServiceActive { get; set; }

        List<MasterPageItem> _navigations;
        public List<MasterPageItem> Navigations { get => _navigations; set => SetProperty(ref _navigations, value); }
        #endregion
        public HomeMasterDetailPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            User = httpService.GetCurrentUser();
            DriverServiceActive = httpService.GetDriverService();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            try
            {
                SetNavigations();
                CheckPermissions();
                Task.Run(LoadSettings).Wait();

            }
            catch (Exception)
            {

                //throw;
            }
        }

        async Task LoadSettings() =>
            await HttpService.GetGeneralSettings();

        void SetNavigations()
        {
            if (Navigations == null) //It will enter the first time we navigate to the MasterDetail Page (Login or App-reopen)
            {

                //Customer Navigations
                if (User != null && User.Roles != null && User.Roles.Contains("User"))
                {
                    Navigations = new List<MasterPageItem>
                    {
                        new MasterPageItem {
                            Title = "Órdenes ",
                            Icon = new FontAwesomeIcon { Icon = "\uf21c", FontType = 1 },
                            NavigateTo = "/HomeMasterDetailPage/NavigationPage/OrdersTabbedPage?selectedTab=OrdersHistory",
                            IsAbsolute = true
                        },
                        new MasterPageItem {
                            Icon = new FontAwesomeIcon { Icon = "\uf2bd", FontType = 1 },
                            Title = "Mi cuenta",
                            NavigateTo = "/HomeMasterDetailPage/NavigationPage/AccountTabbedPage?selectedTab=AccountPage",
                            IsAbsolute = true
                        },
                        new MasterPageItem {
                            Icon = new FontAwesomeIcon { Icon = "\uf0c0", FontType = 1 },
                            Title = "Nosotros",
                            NavigateTo = "/HomeMasterDetailPage/NavigationPage/OursTabbedPage?createTab=OursPage&createTab=LogisticClientPage&createTab=ContactUsPage&selectedTab=OursPage",
                            IsAbsolute = true
                        },
                        new MasterPageItem {
                            Icon = new FontAwesomeIcon { Icon = "\uf56c", FontType = 1 },
                            Title = "Legal",
                            NavigateTo = "/HomeMasterDetailPage/NavigationPage/LegalTabbedPage?selectedTab=LegalPage",
                            IsAbsolute = true
                        }
                    };
                }

                //Driver Navigations
                if (User != null && User.Roles != null && User.Roles.Contains("Driver"))
                {
                    Navigations = new List<MasterPageItem>
                    {
                        new MasterPageItem {
                            Title = "Órdenes ",
                            Icon = new FontAwesomeIcon { Icon = "\uf21c", FontType = 1  },
                            NavigateTo = "/HomeMasterDetailPage/NavigationPage/OrdersDriverTabbedPage?selectedTab=AvailableOrdersDriverPage",
                            IsAbsolute = true
                        },
                        new MasterPageItem {
                            Title = "Mi Perfil",
                            Icon = new FontAwesomeIcon { Icon = "\uf2bd", FontType = 1  },
                            NavigateTo = "/HomeMasterDetailPage/NavigationPage/AccountDriverTabbedPage?selectedTab=ProfileDriverPage",
                            IsAbsolute = true
                        },
                        new MasterPageItem {
                            Icon = new FontAwesomeIcon { Icon = "\uf571", FontType = 1 },
                            Title = "Estado de cuenta",
                            NavigateTo = "/HomeMasterDetailPage/NavigationPage/FinanceTabbedPage?selectedTab=FinancePending",
                            IsAbsolute = true
                        },
                        new MasterPageItem {
                            Icon = new FontAwesomeIcon { Icon = "\uf0c0", FontType = 1 },
                            Title = "Nosotros",
                            NavigateTo = "/HomeMasterDetailPage/NavigationPage/OursTabbedPage?createTab=OursPage&createTab=LogisticPage&createTab=ContactUsPage&selectedTab=OursPage",
                            IsAbsolute = true
                        },
                        new MasterPageItem {
                            Icon = new FontAwesomeIcon { Icon = "\uf56c", FontType = 1 },
                            Title = "Legal",
                            NavigateTo = "/HomeMasterDetailPage/NavigationPage/LegalDriverTabbedPage?selectedTab=TermsAndConditionsPage",
                            IsAbsolute = true
                        }
                    };
                }

                //Common Navigations
                Navigations.Add(new MasterPageItem
                {
                    Title = "Cerrar Sesión",
                    Icon = new FontAwesomeIcon { Icon = "\uf2f5", FontType = 1 },
                    NavigateTo = "/NavigationPage/LoginPage",
                    IsAbsolute = true
                });

                //Filling up the Item Command
                Navigations = Navigations.Select(x =>
                {
                    if (x.IsAbsolute)
                    {
                        //In case of being absolute, we use the "Absolute navigation"
                        x.NavigateToCommand = new DelegateCommand(async () =>
                        {
                            if (x.NavigateTo == "/NavigationPage/LoginPage")
                                HttpService.Logout();
                            await NavigationService.NavigateAsync(new Uri(x.NavigateTo, UriKind.Absolute));
                        });
                    }
                    else
                    {
                        //In case of not being absolute, we use the basic Navigation
                        x.NavigateToCommand = new DelegateCommand(async () =>
                        {
                            await NavigationService.NavigateAsync(x.NavigateTo);
                        });
                    }
                    return x;
                }).ToList();
            }
        }

        async void CheckPermissions()
        {
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
                storageStatus = results[Permission.Location];
            }
        }
    }
}
