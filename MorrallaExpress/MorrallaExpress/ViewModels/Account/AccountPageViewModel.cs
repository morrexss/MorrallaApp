using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using Prism.Commands;
using Prism.Navigation;

namespace MorrallaExpress.ViewModels.Account
{
    public class AccountPageViewModel : ViewModelBase
    {
        public DelegateCommand UpdateInformationCommand { get; set; }

        public UserSession _User = new UserSession();
        public UserSession User
        {
            get => _User;
            set => SetProperty(ref _User, value);
        }

        string _photoUrl = "UserInfo.png";
        public string PhotoUrl
        {
            get => _photoUrl;
            set => SetProperty(ref _photoUrl, value);
        }
        
        public AccountPageViewModel(INavigationService navigationService, IHttpService httpService)
            : base(navigationService, httpService)
        {
            UpdateInformationCommand = new DelegateCommand(Update);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            User = new UserSession();
            User = HttpService.GetCurrentUser();
            PhotoUrl = User.Photo;
        }

        public async void Update()
        {
            await NavigationService.NavigateAsync("EditProfilePage");
            User = HttpService.GetCurrentUser();
            PhotoUrl = User.Photo;
        }
    } 
}
