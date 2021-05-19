using MorrallaExpress.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MorrallaExpress.ViewModels
{
    public class WelcomePageViewModel : ViewModelBase
    {
        int _position = 0;
        public int Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }
        ObservableCollection<CarouselList> _carouselList = new ObservableCollection<CarouselList>();
        public ObservableCollection<CarouselList> CarouselLista
        {
            get => _carouselList;
            set => SetProperty(ref _carouselList, value);
        }
        public DelegateCommand OnPositionSelectedCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand ChangePositionCommand { get; set; }
        public WelcomePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            //OnPositionSelectedCommand = new DelegateCommand(OnPositionSelected);
            ChangePositionCommand = new DelegateCommand(ChangePosition);
            GoBackCommand = new DelegateCommand(async () => await NavigateBack());
            ListaCarousel();
        }
        //async void OnPositionSelected() {
        //    await Task.Delay(7000);
        //    if (Position < 3) Position++;
        //}

        public void ChangePosition()
        {
            if (Position < 3) Position++;
        }
        public void ListaCarousel()
        {
            CarouselLista.Add(new CarouselList { Id = 1, Texto = "Realiza una orden", Descripcion = "Para realizar una orden indicanos tu sucursal", Icono = "Map.png", Fondo = "FondoMorralla.png", BackCommand = GoBackCommand });
            CarouselLista.Add(new CarouselList { Id = 2, Texto = "Selecciona tu morralla", Descripcion = "Selecciona la morralla que necesites", Icono = "Coins.png", Fondo = "FondoMorralla.png",  BackCommand = GoBackCommand });
            CarouselLista.Add(new CarouselList { Id = 3, Texto = "Confirma tu orden", Descripcion = "¡Así de sencillo!", Icono = "Receipt.png", Fondo = "FondoMorralla.png", BackCommand = GoBackCommand });
        }
    }
}
