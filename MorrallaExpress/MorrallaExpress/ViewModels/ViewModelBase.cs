using Acr.UserDialogs;
using MorrallaExpress.Helpers.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MorrallaExpress.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        #region Props
        protected INavigationService NavigationService { get; private set; }
        protected IHttpService HttpService { get; private set; }
        protected IEventAggregator EventService { get; private set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy == value)
                    return;

                isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }
        #endregion

        #region Commands
        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand<string> NavigateWOAnimationCommand { get; private set; }
        public DelegateCommand<string> ModalNavigateCommand { get; private set; }
        public DelegateCommand<string> AbsoluteNavigationCommand { get; private set; }
        public DelegateCommand BackCommand { get; private set; }
        public DelegateCommand PopToRootCommand { get; private set; }
        #endregion

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
            Initialize();
        }

        public ViewModelBase(INavigationService navigationService, IHttpService httpService)
        {
            NavigationService = navigationService;
            HttpService = httpService;
            Initialize();
        }

        public ViewModelBase(INavigationService navigationService, IHttpService httpService, IEventAggregator eventService)
        {
            NavigationService = navigationService;
            HttpService = httpService;
            EventService = eventService;
            Initialize();
        }

        void Initialize()
        {
            // NavigateCommand = new DelegateCommand<string>();
            // AbsoluteNavigationCommand = new DelegateCommand<string>();
            // BackCommand = new DelegateCommand();
            // PopToRootCommand = new DelegateCommand();
            // ModalNavigateCommand = new DelegateCommand<string>();
            // NavigateWOAnimationCommand = new DelegateCommand<string>();
            AcceptCommand = new DelegateCommand(HandleAcceptCommand);
            CancelCommand = new DelegateCommand(HandleCancelCommand);
        }

        #region Navigation Methods
        public async Task Navigate(string navigateTo)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await NavigationService.NavigateAsync(navigateTo);
                IsBusy = false;
            }
        }

        public async Task Navigate(string navigateTo, NavigationParameters param)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await NavigationService.NavigateAsync(navigateTo, param);
                IsBusy = false;
            }
        }

        public async Task NavigateWOAnimation(string navigateTo)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await NavigationService.NavigateAsync(navigateTo, null, null, false);
                IsBusy = false;
            }
        }

        public async Task ModalNavigate(string navigateTo)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await NavigationService.NavigateAsync(navigateTo, null, true);
                IsBusy = false;
            }
        }

        public async Task NavigateBack(NavigationParameters param = null)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await NavigationService.GoBackAsync(param);
                IsBusy = false;
            }
        }

        public async Task PopToRoot()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await NavigationService.GoBackToRootAsync();
                IsBusy = false;
            }
        }

        public async Task AbsoluteNavigation(string navigateTo)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                await NavigationService.NavigateAsync(new Uri(navigateTo, UriKind.Absolute));
                IsBusy = false;
            }
        }
        #endregion

        #region LifeCycle
        public async virtual void Initialize(INavigationParameters parameters)
        {
            await Task.Run(() => { });
        }

        public async virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
            await Task.Run(() => { });
        }

        public async virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            await Task.Run(() => { });

        }

        public async virtual void Destroy()
        {
            await Task.Run(() => { });
        }
        #endregion

        #region PopUpSection
        DelegateCommand _acceptCommand;
        DelegateCommand _acceptCallback;
        DelegateCommand _cancelCommand;
        DelegateCommand _cancelCallback;
        public DelegateCommand AcceptCommand
        {
            get => _acceptCommand;
            set => SetProperty(ref _acceptCommand, value);
        }

        public DelegateCommand AcceptCallback
        {
            get => _acceptCallback;
            set => SetProperty(ref _acceptCallback, value);
        }

        public DelegateCommand CancelCommand
        {
            get => _cancelCommand;
            set => SetProperty(ref _cancelCommand, value);
        }

        public DelegateCommand CancelCallback
        {
            get => _cancelCallback;
            set => SetProperty(ref _cancelCallback, value);
        }

        public void HandleAcceptCommand()
        {
            if (AcceptCallback != null && AcceptCallback.CanExecute())
                AcceptCallback.Execute();
        }

        public void HandleCancelCommand()
        {
            if (CancelCallback != null && CancelCallback.CanExecute())
                CancelCallback.Execute();
        }

        public async Task PopUp(string title = "", string message = "", string accept = "", DelegateCommand acceptCommand = null, string cancel = null, DelegateCommand cancelCommand = null)
        {
            if (string.IsNullOrEmpty(accept))
                accept = "Ok";

            if (cancel == null) //Preguntamos si habrá botón de cancelar.
            {
                if (acceptCommand != null) //Preguntamos si hay comando de aceptación (si queremos que pase algo)
                {
                    AcceptCallback = acceptCommand; //En caso de haberlo, lo igualamos al Handler y lo ejecutamos.
                    await UserDialogs.Instance.AlertAsync(message, title, accept);
                    HandleAcceptCommand();
                }
                else //En caso de que no haya, simplemente mostramos y se va a cerrar la alerta.
                    await UserDialogs.Instance.AlertAsync(message, title, accept);
            }
            else //En caso de haber un botón de cancelar (segundo botón)
            {
                if (cancelCommand != null && acceptCommand != null) //Caso 1: Cada botón (2) tendrá su acción.
                {
                    AcceptCallback = acceptCommand;
                    CancelCallback = cancelCommand;
                    bool response = await UserDialogs.Instance.ConfirmAsync(message, title, accept, cancel);
                    if (response)
                        HandleAcceptCommand();
                    else
                        HandleCancelCommand();
                }
                else if (cancelCommand == null && acceptCommand != null) //Caso 2: El Botón cancelar cerrará la alerta mientras el de aceptar tendra su acción
                {
                    AcceptCallback = acceptCommand;
                    bool response = await UserDialogs.Instance.ConfirmAsync(message, title, accept, cancel);
                    if (response)
                        HandleAcceptCommand();
                }
                else if (cancelCommand != null && acceptCommand == null) //Caso 3: El Botón aceptar cerrará la alerta mientras el de cancelar tendra su acción
                {
                    CancelCallback = cancelCommand;
                    bool response = await UserDialogs.Instance.ConfirmAsync(message, title, accept, cancel);
                    if (response)
                        HandleCancelCommand();
                }
                else //Caso 4: Estarán los 2 botones, sin embargo, ninguno tendrá accion (En teoría no deberia llegar aquí, pero da =)
                    await UserDialogs.Instance.AlertAsync(message, title, accept);
            }
        }
        #endregion

        public async Task<bool> CheckForInternet()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
                return true;

            await ShowInternetPopup();
            return false;
        }

        public async Task ShowInternetPopup() =>
            await PopUp("¡Oops!", "¿Estás seguro que estás conectado a internet? Revisa tu conexión.", "Aceptar");

        public async Task ErrorPopUp() =>
            await PopUp("Algo salió mal.", string.IsNullOrEmpty(Services.HttpService._errorString) ? $"Parece que algo salió mal." : Services.HttpService._errorString, "Ok");

        public async Task WarningPopUp() =>
           await PopUp("Algo salió mal.", string.IsNullOrEmpty(Services.HttpService._errorString) ? $"Parece que algo salió mal." : Services.HttpService._errorString, "Ok");

    }
}
