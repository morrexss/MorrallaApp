using Acr.UserDialogs;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using MorrallaExpress.Models.DTOs;
using MorrallaExpress.Validations;
using MorrallaExpress.Validations.Rules;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MorrallaExpress.ViewModels.Account
{
    public class EditProfilePageViewModel : ViewModelBase
    {
        #region Commands
        public DelegateCommand UpdateCommand { get; set; }
        public DelegateCommand SelectPictureCommand { get; set; }
        public DelegateCommand ChangePasswordCommand { get; set; }

        #endregion

        #region Props
        readonly static string _requiredValidationMessage = "* Campo requerido";

        ImageSource _displayImage = ImageSource.FromFile("UserInfo.png");
        public ImageSource DisplayImage { get => _displayImage; set => SetProperty(ref _displayImage, value); }
        public bool HasPicture { get; set; }

        public StateModel[] _states = new StateModel[] { };
        public StateModel[] States
        {
            get => _states;
            set => SetProperty(ref _states, value);
        }

        ValidatableObject<string> _email = new ValidatableObject<string>();
        public ValidatableObject<string> Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
       
        ValidatableObject<string> _name = new ValidatableObject<string>();
        public ValidatableObject<string> Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        ValidatableObject<string> _lastName = new ValidatableObject<string>();
        public ValidatableObject<string> LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        ValidatableObject<string> _phone = new ValidatableObject<string>();
        public ValidatableObject<string> Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }
        #endregion

        public EditProfilePageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            Title = "Editar información";
            UpdateCommand = new DelegateCommand(UpdateInformation);
            SelectPictureCommand = new DelegateCommand(ChooseProfilePicture);
            ChangePasswordCommand = new DelegateCommand(ChangePass);

            AddValidations();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            using (UserDialogs.Instance.Loading("Cargando..."))
            {
                var user = HttpService.GetCurrentUser();

                Email.Value = user.Email;
                Name.Value = user.Name;
                LastName.Value = user.LastName;
                Phone.Value = user.MobilePhone;
            }
        }

        public async void UpdateInformation()
        {
            if (!InfoValidation())
                return;

            var currentUser = HttpService.GetCurrentUser();

            currentUser.Photo = DisplayImage?.ToBase64();
            currentUser.Name = Name.Value;
            currentUser.LastName = LastName.Value;
            currentUser.MobilePhone = Phone.Value;

            var res = false;
            using (UserDialogs.Instance.Loading("Cargando..."))
                res = await HttpService.UpdateUserData(currentUser);
            if (res)
                await NavigationService.GoBackAsync();
            else
                await ErrorPopUp();
        }

        #region ProfilePicture
        public async void ChooseProfilePicture()
        {
            var Picture = await ChoosePicture();
            DisplayImage = Picture;
            if (DisplayImage.IsEmpty)
                DisplayImage = ImageSource.FromFile("UserInfo.png");
        }
        public async Task<string> ChoosePicture()
        {
            string[] options = { "Cámara", "Galería" };
            var action = await UserDialogs.Instance.ActionSheetAsync("Completar acción con", "Cancel", null, null, options);

            if (action.Equals("Galería"))
            {
                var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (storageStatus != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                    storageStatus = results[Permission.Storage];
                }
                if (storageStatus == PermissionStatus.Granted)
                {
                    var src = await PicturePick();
                    HasPicture = true;
                    return src;
                }
                else if (storageStatus != PermissionStatus.Unknown)
                    await PopUp("Permisos Requeridos", "Se necesitan permisos de almacenamiento para continuar", "Ok");
            }

            if (action.Equals("Cámara"))
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (status != PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                    status = results[Permission.Camera];
                }
                if (status == PermissionStatus.Granted)
                {
                    var src = await PictureTake();
                    HasPicture = true;
                    return src;
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await PopUp("Permisos Requeridos", "Se necesitan permisos de Camara para continuar", "Ok", new DelegateCommand(() =>
                    {
                        //If the user accepted and we're on iOS, we open App settings so the user can add permissions
                        if (Device.RuntimePlatform == Device.iOS)
                            CrossPermissions.Current.OpenAppSettings();
                    }), "Más tarde");

                }
            }
            HasPicture = false;
            return null;
        }

        public async Task<string> PictureTake()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                return null;

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Morralla",
                Name = "photo.jpg"
            });

            if (file == null)
                return null;

            //We display the image giving feedback to the user that his photo is gonna upload.
            DisplayImage = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

            return file.Path;
        }

        public async Task<string> PicturePick()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                return null;
            var File = await CrossMedia.Current.PickPhotoAsync();
            if (File == null)
                return null;
            //We display the image giving feedback to the user that his photo is gonna upload.
            DisplayImage = ImageSource.FromStream(File.GetStream);
            return File.Path;
        }
        public async void ChangePass()
        {
            await NavigationService.NavigateAsync("ChangePasswordPage");
        }
        #endregion

        #region -  VALIDATIONS  -
        void AddValidations()
        {
            _name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _lastName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            //_phone.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
        }

        bool InfoValidation()
        {
            Name.IsValid = _name.Validate();
            LastName.IsValid = _lastName.Validate();
            //Phone.IsValid = _phone.Validate();

            return Name.IsValid && LastName.IsValid;
        }
        #endregion
    }

    public static class DisplayImageExtensions
    {
        public static string ToBase64(this ImageSource photo)
        {
            var photoPath = photo.ToString().Replace("File: ", "");
            if (photoPath == "UserInfo.png") return null;
            var bytes = File.ReadAllBytes(photoPath);
            return Convert.ToBase64String(bytes);
        }
    }
}
