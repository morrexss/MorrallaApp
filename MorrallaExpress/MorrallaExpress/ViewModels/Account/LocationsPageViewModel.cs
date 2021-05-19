using MorrallaExpress.Validations;
using MorrallaExpress.Validations.Rules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using MorrallaExpress.Helpers.Interfaces;
using System.Collections.ObjectModel;
using MorrallaExpress.Models;
using MorrallaExpress.Extensions;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace MorrallaExpress.ViewModels.Account
{
    public class LocationsPageViewModel : ViewModelBase
    {
        #region Commands
        public DelegateCommand<LocationModel> ToDetailCommand { get; set; }
        public DelegateCommand RefreshListCommand { get; set; }
        #endregion

        #region Props
        public bool _emptyView = false;
        public bool EmptyView { get => _emptyView; set => SetProperty(ref _emptyView, value); }

        public bool _lista = true;
        public bool Lista { get => _lista; set => SetProperty(ref _lista, value); }
        bool _isListRefreshing = false;
        public bool IsListRefreshing
        {
            get => _isListRefreshing;
            set => SetProperty(ref _isListRefreshing, value);
        }
        #endregion
        public ObservableCollection<LocationModel> Locations { get; set; }

        public LocationsPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            Locations = httpService.GetCurrentLocations().ToObservableCollection();
            ToDetailCommand = new DelegateCommand<LocationModel>(async (model) => await ToDetail(model));
            RefreshListCommand = new DelegateCommand(RefreshList);

            Title = "Mis sucursales";
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var locations = await HttpService.GetLocations(true);
            Locations.Clear();
            foreach (var location in locations)
            {
                location.DeleteCommand = new DelegateCommand<LocationModel>(Delete);
                Locations.Add(location);
            }
            if (Locations.Count == 0)
            {
                EmptyView = true;
                Lista = false;
            }
            else
            {
                EmptyView = false;
                Lista = true;
            }
        }

        public override async void OnNavigatedFrom(INavigationParameters parameters) =>
            await Task.Run(RefreshList);

        async void RefreshList()
        {
            IsListRefreshing = true;
            var locations = await HttpService.GetLocations(true);
            Locations.Clear();
            foreach (var location in locations)
            {
                location.DeleteCommand = new DelegateCommand<LocationModel>(Delete);
                Locations.Add(location);
            }
            IsListRefreshing = false;
        }

        async void Delete(LocationModel modelo)
        {

            var deleteCmd = new DelegateCommand(async () =>
            {
                using (UserDialogs.Instance.Loading("Cargando..."))
                {
                    if (!await HttpService.DeleteLocation(modelo.LocationId))
                        await PopUp("Error", "No se pudo eliminar la sucursal, intente de nuevo.", "Aceptar");
                    else await NavigateBack();
                }
            });
            await PopUp("Eliminar sucursal", $"Estás seguro que deseas eliminar la sucursal {modelo.Name}?",
                "Aceptar", deleteCmd, "Cancelar", null);

        }
        async Task ToDetail(LocationModel model) =>
            await Navigate("LocationDetailPage", new NavigationParameters { { "model", model } });
    }
}
