using Prism.Commands;
using System;
using System.Collections.Generic;
using Prism.Navigation;
using MorrallaExpress.Validations;
using Xamarin.Forms;
using MorrallaExpress.Validations.Rules;
using Acr.UserDialogs;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using MorrallaExpress.Models;
using System.Threading.Tasks;
using Plugin.Media;
using MorrallaExpress.Models.DTOs;
using MorrallaExpress.Helpers.Interfaces;
using System.IO;
using MorrallaExpress.Views.Login.Templates;
using System.Linq;
using Plugin.FirebasePushNotification;
using System.Text.RegularExpressions;

namespace MorrallaExpress.ViewModels.Login
{
    public class RegisterClientPageViewModel : ViewModelBase
    {
        #region Commands
        public DelegateCommand ToStep2Command { get; set; }
        public DelegateCommand ToStep3Command { get; set; }
        public DelegateCommand SelectPictureCommand { get; set; }
        public DelegateCommand LastStepCommand { get; set; }
        public DelegateCommand RfcFocusedCommand { get; set; }
        #endregion

        #region Props
        readonly static string _requiredValidationMessage = "* Campo requerido";
        readonly static string _errorAlertMessageTitle = "Error";
        readonly static string _warningAlertMessageTitle = "Verifique sus datos";
        readonly static string _successAlertMessageTitle = "Éxito!";
        readonly static string _invalidValidationAlertMessage = "Para continuar por favor llene los campos requeridos correctamente.";
        public List<DataTemplate> RegisterClientTemplates { get; set; }
        string _stepTitle;
        public string StepTitle
        {
            get => _stepTitle;
            set => SetProperty(ref _stepTitle, value);
        }
        int _step = 0;
        public int Step
        {
            get => _step;
            set => SetProperty(ref _step, value);
        }
        public bool HasPicture { get; set; }
        ImageSource _displayImage = ImageSource.FromFile("UserInfo.png");
        public ImageSource DisplayImage
        {
            get => _displayImage;
            set => SetProperty(ref _displayImage, value);
        }
        public StateModel[] _states = new StateModel[] { };
        public StateModel[] States
        {
            get => _states;
            set => SetProperty(ref _states, value);
        }
        #region BasicInformation
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
        ValidatableObject<string> _password = new ValidatableObject<string>();
        public ValidatableObject<string> Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        ValidatableObject<string> _confirmpassword = new ValidatableObject<string>();
        public ValidatableObject<string> ConfirmPassword
        {
            get => _confirmpassword;
            set => SetProperty(ref _confirmpassword, value);
        }
        #endregion
        #region CommercialInfo
        ValidatableObject<string> _companyName = new ValidatableObject<string>();
        public ValidatableObject<string> CompanyName
        {
            get => _companyName;
            set => SetProperty(ref _companyName, value);
        }
        ValidatableObject<string> _fiscalName = new ValidatableObject<string>();
        public ValidatableObject<string> FiscalName
        {
            get => _fiscalName;
            set => SetProperty(ref _fiscalName, value);
        }
        ValidatableObject<string> _rfc = new ValidatableObject<string>();
        public ValidatableObject<string> Rfc
        {
            get => _rfc;
            set => SetProperty(ref _rfc, value);
        }
        ValidatableObject<UsoCFDIModel> _usoCFDI = new ValidatableObject<UsoCFDIModel>();
        public ValidatableObject<UsoCFDIModel> UsoCFDI
        {
            get => _usoCFDI;
            set => SetProperty(ref _usoCFDI, value);
        }
        ValidatableObject<string> _fiscalAddress = new ValidatableObject<string>();
        public ValidatableObject<string> FiscalAddress
        {
            get => _fiscalAddress;
            set => SetProperty(ref _fiscalAddress, value);
        }
        ValidatableObject<CityModel> _fiscalCity = new ValidatableObject<CityModel>();
        public ValidatableObject<CityModel> FiscalCity
        {
            get => _fiscalCity;
            set => SetProperty(ref _fiscalCity, value);
        }
        ValidatableObject<StateModel> _fiscalState = new ValidatableObject<StateModel>();
        public ValidatableObject<StateModel> FiscalState
        {
            get => _fiscalState;
            set => SetProperty(ref _fiscalState, value);
        }
        ValidatableObject<string> _fiscalDelegation = new ValidatableObject<string>();
        public ValidatableObject<string> FiscalDelegation
        {
            get => _fiscalDelegation;
            set => SetProperty(ref _fiscalDelegation, value);
        }
        ValidatableObject<string> _fiscalPostalCode = new ValidatableObject<string>();
        public ValidatableObject<string> FiscalPostalCode
        {
            get => _fiscalPostalCode;
            set => SetProperty(ref _fiscalPostalCode, value);
        }
        #region PublicLists
        UsoCFDIModel[] _usoCFDIsList;
        public UsoCFDIModel[] UsoCFDIsList
        {
            get => _usoCFDIsList;
            set => SetProperty(ref _usoCFDIsList, value);
        }
        #endregion
        #endregion
        #region LocationursalInfo
        ValidatableObject<string> _locationName = new ValidatableObject<string>();
        public ValidatableObject<string> LocationName
        {
            get => _locationName;
            set => SetProperty(ref _locationName, value);
        }
        ValidatableObject<string> _locationAddress = new ValidatableObject<string>();
        public ValidatableObject<string> LocationAddress
        {
            get => _locationAddress;
            set => SetProperty(ref _locationAddress, value);
        }
        ValidatableObject<string> _locationExteriorNumber = new ValidatableObject<string>();
        public ValidatableObject<string> LocationExteriorNumber
        {
            get => _locationExteriorNumber;
            set => SetProperty(ref _locationExteriorNumber, value);
        }
        string _locationInteriorNumber;
        public string LocationInteriorNumber
        {
            get => _locationInteriorNumber;
            set => SetProperty(ref _locationInteriorNumber, value);
        }
        ValidatableObject<CityModel> _locationCity = new ValidatableObject<CityModel>();
        public ValidatableObject<CityModel> LocationCity
        {
            get => _locationCity;
            set => SetProperty(ref _locationCity, value);
        }
        ValidatableObject<StateModel> _locationState = new ValidatableObject<StateModel>();
        public ValidatableObject<StateModel> LocationState
        {
            get => _locationState;
            set => SetProperty(ref _locationState, value);
        }
        ValidatableObject<string> _locationDelegation = new ValidatableObject<string>();
        public ValidatableObject<string> LocationDelegation
        {
            get => _locationDelegation;
            set => SetProperty(ref _locationDelegation, value);
        }
        ValidatableObject<string> _locationPostalCode = new ValidatableObject<string>();
        public ValidatableObject<string> LocationPostalCode
        {
            get => _locationPostalCode;
            set => SetProperty(ref _locationPostalCode, value);
        }
        string _authorizedPersonnel;
        public string AuthorizedPersonnel
        {
            get => _authorizedPersonnel;
            set => SetProperty(ref _authorizedPersonnel, value);
        }
        #endregion
        #endregion

        public RegisterClientPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            Title = "Registro de cliente";
            ToStep2Command = new DelegateCommand(ToStep2);
            RfcFocusedCommand = new DelegateCommand(RfcFocused);
            ToStep3Command = new DelegateCommand(ToStep3);
            SelectPictureCommand = new DelegateCommand(SelectPicture);
            LastStepCommand = new DelegateCommand(CreateLocation);
            AddValidations();

            StepTitle = "Información básica";
            RegisterClientTemplates = new List<DataTemplate>
            {
                new DataTemplate(() => { return new BasicFormTemplate { ParentContext = this }; }),
                new DataTemplate(() => { return new FiscalFormTemplate { ParentContext = this }; }),
                new DataTemplate(() => { return new LocationFormTemplate { ParentContext = this };})
            };
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var usos = await HttpService.GetCFDIs();
            UsoCFDIsList = usos.ToArray();
            var states = await HttpService.GetAvailableStates();
            States = states.ToArray();
        }

        #region Actions
        async void ToStep2()
        {
            if (await CheckForInternet() && BasicInfoValidation())
            {
                string caractEspecial = @"[0-9]";
                bool resultado = Regex.IsMatch(Name.Value, caractEspecial, RegexOptions.IgnoreCase);
                bool resultado2 = Regex.IsMatch(LastName.Value, caractEspecial, RegexOptions.IgnoreCase);
                var res = false;
                using (UserDialogs.Instance.Loading("Cargando..."))
                    res = await HttpService.CheckEmail(Email.Value);
                if (res)
                {

                    if (resultado)
                    {
                        await UserDialogs.Instance.AlertAsync("El campo nombre nomás acepta letras", "Error");
                        return;
                    }else if (resultado2)
                    {
                        await UserDialogs.Instance.AlertAsync("Inténtalo de nuevo. El campo de apellidos deben ser letras. ", "Error");
                        return;
                    }
                    StepTitle = "Información comercial";
                    Step++;
                }
                else
                    await PopUp("Correo en uso", "Al parecer este correo ya está en uso, utilice otro.");
            }
            else
                await PopUp(_errorAlertMessageTitle, _invalidValidationAlertMessage);
        }

        async void RfcFocused()
        {
            if (string.IsNullOrEmpty(Rfc.Value))
            {
                var acceptCommand = new DelegateCommand(() => Rfc.Value = "XAXX010101000");
                var alertMsg = "No has especificado tu RFC ¿Quiéres utilizar el genérico?";
                await PopUp("Aviso Importante", alertMsg, "Aceptar", acceptCommand, "Cancelar");
            }
        }

        async void ToStep3()
        {
            if (await CheckForInternet() && FiscalInfoValidation())
            {
                var res = false;
                using (UserDialogs.Instance.Loading("Cargando..."))
                    res = await HttpService.RegisterClient(GenerateRegisterModel());
                if (res)
                {
                    StepTitle = "Agregar sucursal";
                    LocationName.Value = CompanyName.Value;
                    LocationAddress.Value = FiscalAddress.Value;
                    LocationState.Value = States.FirstOrDefault(s => s.StateId == FiscalState.Value.StateId);
                    LocationCity.Value = LocationState.Value
                        .Cities.FirstOrDefault(c => c.CityId == FiscalCity.Value.CityId);
                    LocationPostalCode.Value = FiscalPostalCode.Value;
                    LocationDelegation.Value = FiscalDelegation.Value;
                    Step++;
                }
                else
                    await PopUp(_errorAlertMessageTitle, "Ocurrió un error al registrar tu cuenta, por favor intente de nuevo.");
            }
            else
                await PopUp(_errorAlertMessageTitle, _invalidValidationAlertMessage);
        }

        async void CreateLocation()
        {
            if (await CheckForInternet() && BasicInfoValidation() && FiscalInfoValidation() && LocationValidation())
            {
                var res = false;
                using (UserDialogs.Instance.Loading("Creando..."))
                    res = await HttpService.CreateOrUpdateLocation(GenerateLocationModel());
                if (res)
                {
                    await PopUp(_successAlertMessageTitle, "¡Bienvenido a Morrexss! \n Tu registro se a realizado con éxito.");
                    await AbsoluteNavigation("/HomeMasterDetailPage/NavigationPage/OrdersTabbedPage?selectedTab=PendingOrdersPage");
                }
                else
                    await PopUp("Registro incompleto", "No se pudo completar el registro de sucursal, por favor intente de nuevo.");
            }
            else
                await PopUp(_errorAlertMessageTitle, _invalidValidationAlertMessage);
        }
        #endregion

        #region FieldValidations
        bool BasicInfoValidation()
        {
            var isValidEmail = _email.Validate();
            Email.IsValid = isValidEmail;

            var isValidName = _name.Validate();
            Name.IsValid = isValidName;

            var isValidLastName = _lastName.Validate();
            LastName.IsValid = isValidLastName;

            var isValidPhone = _phone.Validate();
            Phone.IsValid = isValidPhone;

            var isValidPassword = _password.Validate();
            Password.IsValid = isValidPassword;

            var isValidRPassword = _confirmpassword.Validate() && _confirmpassword.Value.Equals(_password.Value);
            ConfirmPassword.IsValid = isValidRPassword;

            return isValidEmail && isValidName && isValidLastName && isValidPhone && isValidPassword && isValidRPassword;
        }

        bool FiscalInfoValidation()
        {
            var isValidCompanyName = _companyName.Validate();
            CompanyName.IsValid = isValidCompanyName;

            var isValidFiscalName = _fiscalName.Validate();
            _fiscalName.IsValid = isValidFiscalName;

            var isValidRfc = _rfc.Validate();
            Rfc.IsValid = isValidRfc;

            var isValidUsoCfdi = _usoCFDI.Validate();
            UsoCFDI.IsValid = isValidUsoCfdi;

            var isValidFiscalAddress = _fiscalAddress.Validate();
            FiscalAddress.IsValid = isValidFiscalAddress;

            var isValidFiscalCity = _fiscalCity.Validate();
            FiscalCity.IsValid = isValidFiscalCity;

            var isValidFiscalState = _fiscalState.Validate();
            FiscalState.IsValid = isValidFiscalState;

            var isValidFiscalPostalCode = _fiscalPostalCode.Validate();
            _fiscalPostalCode.IsValid = isValidFiscalPostalCode;

            return isValidCompanyName && isValidFiscalName && isValidRfc && isValidUsoCfdi &&
                isValidFiscalAddress && isValidFiscalCity && isValidFiscalState &
                isValidFiscalPostalCode;
        }

        bool LocationValidation()
        {
            var isValidLocationName = _locationName.Validate();
            LocationName.IsValid = isValidLocationName;

            var isValidLocationAddress = _locationAddress.Validate();
            LocationAddress.IsValid = isValidLocationAddress;

            var isValidLocationExteriorNumber = _locationExteriorNumber.Validate();
            LocationExteriorNumber.IsValid = isValidLocationExteriorNumber;

            var isValidLocationCity = _locationCity.Validate();
            LocationCity.IsValid = isValidLocationCity;

            var isValidLocationState = _locationState.Validate();
            LocationState.IsValid = isValidLocationState;

            var isValidLocationDelegation = _locationDelegation.Validate();
            LocationDelegation.IsValid = isValidLocationDelegation;

            var isValidLocationPostalCode = _locationPostalCode.Validate();
            LocationPostalCode.IsValid = isValidLocationPostalCode;

            return isValidLocationName && isValidLocationAddress && isValidLocationExteriorNumber &&
                isValidLocationCity && isValidLocationState && isValidLocationPostalCode &&
                isValidLocationDelegation;
        }

        void AddValidations()
        {
            #region BasicInfo
            _email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _email.Validations.Add(new EmailRule());
            _name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _lastName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });

            _phone.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _password.Validations.Add(new PasswordRule());
            _confirmpassword.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _confirmpassword.Comparison.Add(new ComparePasswordsRule { ValidationMessage = "Las contraseñas no coinciden", ValueToCompare = Password });
            #endregion

            #region CommercialInfo
            _companyName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _fiscalName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _rfc.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _usoCFDI.Validations.Add(new IsNotNullOrEmptyRule<UsoCFDIModel> { ValidationMessage = _requiredValidationMessage });
            _fiscalAddress.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _fiscalCity.Validations.Add(new IsNotNullOrEmptyRule<CityModel> { ValidationMessage = _requiredValidationMessage });
            _fiscalState.Validations.Add(new IsNotNullOrEmptyRule<StateModel> { ValidationMessage = _requiredValidationMessage });
            _fiscalDelegation.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _fiscalPostalCode.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            #endregion

            #region LocationursalInfo
            _locationName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _locationAddress.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _locationExteriorNumber.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _locationCity.Validations.Add(new IsNotNullOrEmptyRule<CityModel> { ValidationMessage = _requiredValidationMessage });
            _locationState.Validations.Add(new IsNotNullOrEmptyRule<StateModel> { ValidationMessage = _requiredValidationMessage });
            _locationDelegation.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            _locationPostalCode.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = _requiredValidationMessage });
            #endregion
        }
        #endregion

        #region AuthPersonnelPhoto
        public async void SelectPicture()
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
                if (storageStatus != PermissionStatus.Granted )
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage});
                    storageStatus = results[Permission.Storage];
                }
                if (storageStatus == PermissionStatus.Granted)
                {
                    var src = await PicturePick();
                    HasPicture = true;
                    return src;
                }
                else if (storageStatus !=  PermissionStatus.Unknown)
                    await PopUp("Permisos Requeridos", "Se necesitan permisos de almacenamiento para continuar", "Ok");
            }

            if (action.Equals("Cámara"))
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (status != PermissionStatus.Granted )
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera});
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
                    }),"Más tarde");
                    
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
        #endregion

        RegisterClientRequestModel GenerateRegisterModel() => new RegisterClientRequestModel
        {
            Email = Email.Value,
            Name = Name.Value,
            LastName  = LastName.Value,
            MobilePhone = Phone.Value,
            CompanyName = CompanyName.Value,
            Password = Password.Value,
            ConfirmPassword = ConfirmPassword.Value,
            Rfc = Rfc.Value,
            UsoId = UsoCFDI.Value.Id,
            FiscalName = FiscalName.Value,
            FiscalAddress = FiscalAddress.Value,
            FiscalState = FiscalState.Value.Name,
            FiscalCity = FiscalCity.Value.Name,
            FiscalPostalCode = FiscalPostalCode.Value,
            //FiscalDelegationName = FiscalDelegation.Value,
            FireBaseToken = CrossFirebasePushNotification.Current.Token
        };
        LocationModel GenerateLocationModel() => new LocationModel
        {
            Name = LocationName.Value,
            Address1 = LocationAddress.Value,
            ExteriorNumber = LocationExteriorNumber.Value,
            InteriorNumber = LocationInteriorNumber,
            CityId = LocationCity.Value.CityId,
            StateId = LocationState.Value.StateId,
            PostalCode = LocationPostalCode.Value,
            Photo = DisplayImage.ToBase64(),
            SubdivisionName = LocationDelegation.Value,
            AuthorizedPersonel = AuthorizedPersonnel
        };
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