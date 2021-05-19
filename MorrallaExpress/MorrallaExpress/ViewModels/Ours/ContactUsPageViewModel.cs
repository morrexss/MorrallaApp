using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MorrallaExpress.ViewModels.Ours
{
    public class ContactUsPageViewModel : ViewModelBase
    {

        string _telMorrexss;
        public string TelMorrexss { get => _telMorrexss; set => SetProperty(ref _telMorrexss, value); }

        string _webMorrexss;
        public string WebMorrexss { get => _webMorrexss; set => SetProperty(ref _webMorrexss, value); }

        string _mailMorrexss;
        public string MailMorrexss { get => _mailMorrexss; set => SetProperty(ref _mailMorrexss, value); }

        string _clarificationsMailMorrexss;
        public string ClarificationsMailMorrexss { get => _clarificationsMailMorrexss; set => SetProperty(ref _clarificationsMailMorrexss, value); }

        public DelegateCommand OpenTelCommand { get; private set; }
        public DelegateCommand OpenWebCommand { get; private set; }
        public DelegateCommand OpenMailCommand { get; private set; }
        public DelegateCommand OpenClarificationsMailCommand { get; private set; }

        public ContactUsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            TelMorrexss = "6622108990";
            OpenTelCommand = new DelegateCommand(OpenTel);

            WebMorrexss = "www.morrexss.com";
            OpenWebCommand = new DelegateCommand(OpenWeb);

            MailMorrexss = "atencion@morrexss.com";
            OpenMailCommand = new DelegateCommand(OpenMail);

            ClarificationsMailMorrexss = "aclaraciones@morrexss.com";
            OpenClarificationsMailCommand = new DelegateCommand(OpenClarificationsMail);
        }

        public void OpenTel()
        {
            try
            {
                PhoneDialer.Open(TelMorrexss);
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

        public void OpenWeb()
        {
            Launcher.OpenAsync(new Uri($"http://{WebMorrexss}"));
        }

        public void OpenMail()
        {
            Launcher.OpenAsync(new Uri($"mailto:{MailMorrexss}"));
        }

        public void OpenClarificationsMail()
        {
            Launcher.OpenAsync(new Uri($"mailto:{ClarificationsMailMorrexss}"));
        }

    }
}
