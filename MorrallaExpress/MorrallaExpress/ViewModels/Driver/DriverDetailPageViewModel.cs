using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using MorrallaExpress.Models.DTOs;
using MorrallaExpress.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace MorrallaExpress.ViewModels
{
    public class DriverDetailPageViewModel : ViewModelBase
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

        private string _gold;
        public string Gold
        {
            get => _gold;
            set => SetProperty(ref _gold, value);
        }

        private string _silver;
        public string Silver
        {
            get => _silver;
            set => SetProperty(ref _silver, value);
        }

        private string _bronze;
        public string Bronze
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

        public DriverDetailPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            OpenTelMobileCommand = new DelegateCommand(OpenTelMobile);
            OpenTelHomeCommand = new DelegateCommand(OpenTelHome);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters["model"] is OrderModel model)
            {
                CurrentOrder = model;
                var driver = model.Driver;
                Name = $"{driver.Name} {driver.LastName}";
                Email = driver.Email;
                Deliveries = driver.DeliveryCount;
                SetYear(driver.DateCreated);
                StartDate = driver.DateCreated.ToString("dd MMMM yyyy");
                if (driver.Rating != null)
                {
                    Gold = driver?.Rating?.Gold.ToString() ?? "0";
                    Silver = driver?.Rating?.Silver.ToString() ?? "0";
                    Bronze = driver?.Rating?.Bronze.ToString() ?? "0";
                }
            }
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
