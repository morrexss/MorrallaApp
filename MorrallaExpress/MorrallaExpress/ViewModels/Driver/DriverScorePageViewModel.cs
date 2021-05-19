using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models.DTOs;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace MorrallaExpress.ViewModels
{
    public class DriverScorePageViewModel : ViewModelBase
    {
        #region Props
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private int _deliveries;
        public int Deliveries
        {
            get => _deliveries;
            set => SetProperty(ref _deliveries, value);
        }

        private int _gold;
        public int Gold
        {
            get => _gold;
            set => SetProperty(ref _gold, value);
        }

        private int _silver;
        public int Silver
        {
            get => _silver;
            set => SetProperty(ref _silver, value);
        }

        private int _bronze;
        public int Bronze
        {
            get => _bronze;
            set => SetProperty(ref _bronze, value);
        }

        private string _years;
        public string Years
        {
            get => _years;
            set => SetProperty(ref _years, value);
        }

        public string _startDate;
        public string StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private OrderModel _currentOrder;
        public OrderModel CurrentOrder
        {
            get { return _currentOrder; }
            set { SetProperty(ref _currentOrder, value); }
        }
        #endregion
        public DelegateCommand OpenTelMobileCommand { get; private set; }
        public DelegateCommand OpenTelHomeCommand { get; private set; }

        public DriverScorePageViewModel(INavigationService navigationService, IHttpService httpService, IEventAggregator eventService) 
            : base(navigationService, httpService, eventService)
        {
            OpenTelMobileCommand = new DelegateCommand(OpenTelMobile);
            OpenTelHomeCommand = new DelegateCommand(OpenTelHome);
            var driver = httpService.GetCurrentUser();
            Name = $"{driver.Name} {driver.LastName}";
            Email = driver.Email;
            Deliveries = driver.DeliveryCount;
            if (driver.Rating != null)
            {
                Gold = driver.Rating.Gold;
                Silver = driver.Rating.Silver;
                Bronze = driver.Rating.Bronze;
            }

            SetYear(driver.DateCreated);

            StartDate = driver.DateCreated.ToString("dd MMMM yyyy");
        }

        public void SetYear(DateTime creationDate)
        {
            var res = DateTime.Now - creationDate;
            if (res.Days >= 365)
            {
                var years = res.Days / 365;
                if (years > 1)
                    Years = $"{years} años";
                else
                    Years = $"1 año";
            }
            else
            {
                var months = res.Days / 12;
                if (months > 0)
                {
                    if (months > 1)
                        Years = $"{months} meses";
                    else
                        Years = $"{months} mes";
                }
                else
                {
                    if (res.Days > 1)
                        Years = $"{res.Days} días";
                    else
                        Years = $"1 día";
                }
            }
        }

        public void OpenTelMobile()
        {
            string number = CurrentOrder.Driver.MobilePhone;
            if (!string.IsNullOrEmpty(number))
                Call(number);
        }

        public void OpenTelHome()
        {
            string number = CurrentOrder.Driver.HomePhone;
            if (!string.IsNullOrEmpty(number))
                Call(number);
        }

        public void Call(string number)
        {
            try
            {
                PhoneDialer.Open(number);
            }
            catch (FeatureNotSupportedException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
