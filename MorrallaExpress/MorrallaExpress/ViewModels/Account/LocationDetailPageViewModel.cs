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
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace MorrallaExpress.ViewModels.Account
{
    public class LocationDetailPageViewModel : ViewModelBase
    {
        #region Commands
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        #endregion

        #region Props
        LocationModel _selected;
        bool _isEdit;
        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }

       
        string _deleteIconSource = "DeleteIcn.png";
        public string DeleteIconSource
        {
            get => _deleteIconSource;
            set => SetProperty(ref _deleteIconSource, value);
        }
        public StateModel[] _states = { };
        public StateModel[] States
        {
            get => _states;
            set => SetProperty(ref _states, value);
        }
        // -------- Mover a AuthorizedPersonnelModel
        ImageSource _displayImage = ImageSource.FromFile("UserInfo.png");
        public ImageSource DisplayImage { get => _displayImage; set => SetProperty(ref _displayImage, value); }
        public bool HasPicture { get; set; }
        // -----------------------------
        ValidatableObject<string> _name = new ValidatableObject<string>();
        public ValidatableObject<string> Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        ValidatableObject<string> _street1 = new ValidatableObject<string>();
        public ValidatableObject<string> Street1
        {
            get => _street1;
            set => SetProperty(ref _street1, value);
        }
        ValidatableObject<string> _exteriorNumber = new ValidatableObject<string>();
        public ValidatableObject<string> ExteriorNumber
        {
            get => _exteriorNumber;
            set => SetProperty(ref _exteriorNumber, value);
        }
        string _interiorNumber;
        public string InteriorNumber
        {
            get => _interiorNumber;
            set => SetProperty(ref _interiorNumber, value);
        }
        ValidatableObject<StateModel> _state = new ValidatableObject<StateModel>();
        public ValidatableObject<StateModel> State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
        ValidatableObject<CityModel> _city = new ValidatableObject<CityModel>();
        public ValidatableObject<CityModel> City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }
        ValidatableObject<string> _delegation = new ValidatableObject<string>();
        public ValidatableObject<string> Delegation
        {
            get => _delegation;
            set => SetProperty(ref _delegation, value);
        }
        ValidatableObject<string> _postalCode = new ValidatableObject<string>();
        public ValidatableObject<string> PostalCode
        {
            get => _postalCode;
            set => SetProperty(ref _postalCode, value);
        }
        AuthorizedPersonnelModel[] _authorizedPersonnel = { new AuthorizedPersonnelModel { Photo = "UserInfo.png" } };
        public AuthorizedPersonnelModel[] AuthorizedPersonnel
        {
            get => _authorizedPersonnel;
            set => SetProperty(ref _authorizedPersonnel, value);
        }

        Position _position;
        public Position Position { get => _position; set => SetProperty(ref _position, value); }


        public Map _MyMap;
        public Map MyMap { get => _MyMap; set => SetProperty(ref _MyMap, value); }

        #endregion

        public LocationDetailPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            SaveCommand = new DelegateCommand(Save);
            DeleteCommand = new DelegateCommand(Delete);

            AddValidations();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            using (UserDialogs.Instance.Loading("Cargando..."))
            {
                States = HttpService.GetCurrentStates();
                if (!States.Any())
                    States = (await HttpService.GetAvailableStates()).ToArray();
                if (parameters["model"] is LocationModel model)
                {
                    IsEdit = true;
                    _selected = model;
                    Name.Value = model.Name;
                    Street1.Value = model.Address1;
                    ExteriorNumber.Value = model.ExteriorNumber;
                    InteriorNumber = model.InteriorNumber;
                    PostalCode.Value = model.PostalCode;
                    Delegation.Value = model.SubdivisionName;
                    if (model.StateId != null)
                        State.Value = States.FirstOrDefault(s => s.StateId == model.StateId);
                    if (model.CityId != null)
                        City.Value = State.Value
                            .Cities.FirstOrDefault(c => c.CityId == model.CityId);
                    AuthorizedPersonnel = model.AuthorizedPersonnel;

                    // MAPA
                    if (model.Latitude.HasValue && model.Longitude.HasValue)
                    {
                        _position = new Position(model.Latitude.GetValueOrDefault(), model.Longitude.GetValueOrDefault());

                        Position locationPosition = _position;
                        string locationName = Name.Value;

                        if (locationPosition != null && locationPosition.Latitude != 0 && locationPosition.Longitude != 0)
                        {
                            Pin pin = new Pin
                            {
                                Label = locationName,
                                Type = PinType.Place,
                                Position = locationPosition
                            };
                            MyMap.Pins.Add(pin);
                        }
                        // iniciamos el mapa en el PIN anterior o posición actual
                        MyMap.MoveToRegion(new MapSpan(locationPosition, 0.01, 0.01));
                    }
                }
                else
                {
                    IsEdit = false;
                }
                    
            }
        }

        #region Actions
        async void Delete()
        {

            var deleteCmd = new DelegateCommand(async () =>
            {
                using (UserDialogs.Instance.Loading("Cargando..."))
                {
                    if (!await HttpService.DeleteLocation(_selected.LocationId))
                        await PopUp("Error", "No se pudo eliminar la sucursal, intente de nuevo.", "Aceptar");
                    else await NavigateBack();
                }
            });
            await PopUp("Eliminar sucursal", $"Estás seguro que deseas eliminar la sucursal {_selected.Name}?",
                "Aceptar", deleteCmd, "Cancelar", null);
        }

        async void Save()
        {
            if (!Validate())
            {
                await PopUp("Te hacen falta datos", "Por favor revisa que todos los campos estén completos.");
                return;
            }

            // VALIDA MAPA
            if (IsEdit)
            {
                if (MyMap.Pins.Count == 1)
                    _position = new Position(MyMap.Pins[0].Position.Latitude, MyMap.Pins[0].Position.Longitude);
                else
                {
                    await PopUp("Error", $"Debe marcar en el mapa la ubicación de la sucursal.", "Aceptar");
                    return;
                }
            }

            using (UserDialogs.Instance.Loading("Cargando..."))
            {
                var res = await HttpService.CreateOrUpdateLocation(GenerateLocation(), !IsEdit);
                if (!res)
                {
                    var action = IsEdit ? "editar" : "crear";
                    await PopUp("Error", $"No se pudo {action} la sucursal, intente de nuevo.", "Aceptar");
                }
            }
            await NavigationService.NavigateAsync(new
                        Uri("/HomeMasterDetailPage/NavigationPage/AccountTabbedPage?selectedTab=LocationsPage",
                        UriKind.Absolute));
            //await PopToRoot();
            //if (IsEdit)
            //    await NavigateBack();
            //else
            //{
            //    var locations = HttpService.GetCurrentLocations();
            //    var locationNew = locations.Last();
            //    await Navigate("LocationDetailPage", new NavigationParameters { { "model", locationNew } });
            //}
        }
        #endregion

        #region Validations
        bool Validate()
        {
            var isValidName = _name.Validate();
            Name.IsValid = isValidName;

            var isValidStreet1 = _street1.Validate();
            Street1.IsValid = isValidStreet1;

            var isValidExteriorNumber = _exteriorNumber.Validate();
            ExteriorNumber.IsValid = isValidExteriorNumber;

            var isValidCity = _city.Validate();
            City.IsValid = isValidCity;

            var isValidState = _state.Validate();
            State.IsValid = isValidState;

            var isValidPostalCode = _postalCode.Validate();
            PostalCode.IsValid = isValidPostalCode;

            var isValidDelegation = _delegation.Validate();
            Delegation.IsValid = isValidDelegation;

            return isValidName && isValidStreet1 && isValidExteriorNumber && isValidCity &&
                isValidState && isValidPostalCode && isValidDelegation;
        }

        void AddValidations()
        {
            _name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* Campo requerido" });
            _street1.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* Campo requerido" });
            _exteriorNumber.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* Campo requerido" });
            _city.Validations.Add(new IsNotNullOrEmptyRule<CityModel> { ValidationMessage = "* Campo requerido" });
            _state.Validations.Add(new IsNotNullOrEmptyRule<StateModel> { ValidationMessage = "* Campo requerido" });
            _delegation.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* Campo requerido" });
            _postalCode.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* Campo requerido" });
        }
        #endregion

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
        #endregion

        LocationModel GenerateLocation() => new LocationModel
        {
            LocationId = _selected != null ? _selected.LocationId : 0,
            Name = Name.Value,
            Address1 = Street1.Value,
            ExteriorNumber = ExteriorNumber.Value,
            InteriorNumber = InteriorNumber,
            CityId = City.Value.CityId,
            StateId = State.Value.StateId,
            PostalCode = PostalCode.Value,
            SubdivisionName = Delegation.Value,
            ApplicationUserId = HttpService.GetCurrentUser().UserId,
            Latitude = IsEdit ? _position.Latitude : 0,
            Longitude = IsEdit ? _position.Longitude : 0
        };


    }
}
