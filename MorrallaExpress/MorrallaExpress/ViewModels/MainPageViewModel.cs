using MorrallaExpress.Helpers.Interfaces;
using Prism.Navigation;

namespace MorrallaExpress.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService, IHttpService httpService) : base(navigationService, httpService)
        {
           
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            await HttpService.GetGeneralSettings();
        }
    }
}
