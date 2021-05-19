
using MorrallaExpress.Models.DTOs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MorrallaExpress.ViewModels
{
    public class DenominationsPageViewModel : ViewModelBase
    {
        private List<DenominationModel>  _denominations = new List<DenominationModel> { };
        public List<DenominationModel> DenominationsList
        {
            get { return _denominations; }
            set { SetProperty(ref _denominations, value); }
        }

        public DenominationsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
          
        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var list = (List<DenominationModel>)parameters["Denominations"] ?? DenominationsList;
            var toRemove = list.FirstOrDefault(a => a.Value == 20);
            list.Remove(toRemove);
            foreach (var item in list)
            {
                item.Name = item.Name.Replace("$", "");
                item.WeightString = $"{item.Weight} Grs";
            }
            DenominationsList = list;
        }
    }
}
