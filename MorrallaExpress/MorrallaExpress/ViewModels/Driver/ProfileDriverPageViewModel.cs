using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using MorrallaExpress.Models.DTOs;
using MorrallaExpress.Validations;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;

namespace MorrallaExpress.ViewModels
{
    public class ProfileDriverPageViewModel : ViewModelBase
    {
        public DelegateCommand DriverCommand { get; set; }
        public DelegateCommand ChangePasswordCommand { get; set; }

        #region props

        public StartDriverServiceRequestModel _start = new StartDriverServiceRequestModel();

        public StartDriverServiceRequestModel Start
        {
            get => _start;
            set => SetProperty(ref _start, value);
        }

        public StartDriverServiceRequestModel _stop = new StartDriverServiceRequestModel();

        public StartDriverServiceRequestModel Stop
        {
            get => _stop;
            set => SetProperty(ref _stop, value);
        }

        public UserSession _driver = new UserSession();
        public UserSession Driver
        {
            get => _driver;
            set => SetProperty(ref _driver, value);
        }

        bool _isActive = false;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        ValidatableObject<string> _name = new ValidatableObject<string>();
        public ValidatableObject<string> Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        ValidatableObject<string> _email = new ValidatableObject<string>();
        public ValidatableObject<string> Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        ValidatableObject<string> _phone = new ValidatableObject<string>();
        public ValidatableObject<string> Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }
        string _photo = "UserInfo.png";
        public string Photo
        {
            get => _photo;
            set => SetProperty(ref _photo, value);
        }
        #endregion
        public ProfileDriverPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            //Title = "Perfil 123";
            var driver = httpService.GetCurrentUser();
            IsActive = httpService.GetDriverService();
            Driver = driver;
            Name.Value = $"{driver.Name} {driver.LastName}";
            Email.Value = driver.Email;
            Phone.Value = driver.MobilePhone;
            if (!string.IsNullOrEmpty(driver.Photo))
            {
                Photo = driver.Photo;
            }
            DriverCommand = new DelegateCommand(Driveerr);
            ChangePasswordCommand = new DelegateCommand(async () => await Navigate("ChangePasswordPage"));
        }

        public async void Driveerr()
        {
            if (IsActive)
            {
                Start.UserId = HttpService.GetCurrentUser().UserId;
                Location pos = null;
                try
                {
                     pos = await Geolocation.GetLastKnownLocationAsync();
                }
                catch (Exception ex)
                {
                    
                }
                if (pos is null)
                {
                    IsActive = false;
                    await PopUp("Error!", "Verifica que los servicios de localización estén encendidos.", "Aceptar");
                    return;
                }
                Start.CurrentLatitude = pos.Latitude;
                Start.CurrentLongitude = pos.Longitude;
                IsActive = await HttpService.StartDriverService(Start);
                if (!IsActive)
                    await PopUp("Error!", "Verifica tu conexión a internet.", "Aceptar");
            }
            else
            {
                Stop.UserId = HttpService.GetCurrentUser().UserId;
                var pos = await Geolocation.GetLastKnownLocationAsync();
                if (pos is null)
                {
                    IsActive = false;
                    Stop.CurrentLatitude = Start.CurrentLatitude;
                    Stop.CurrentLongitude = Start.CurrentLongitude;
                    await HttpService.StopDriverService(Stop);
                    return;
                }
                Stop.CurrentLatitude = pos.Latitude;
                Stop.CurrentLongitude = pos.Longitude;
                await HttpService.StopDriverService(Stop);
            }
        }
    }
}
