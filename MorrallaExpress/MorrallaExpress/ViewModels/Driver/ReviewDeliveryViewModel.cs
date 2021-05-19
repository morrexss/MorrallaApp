using Acr.UserDialogs;
using MorrallaExpress.Helpers.Interfaces;
using MorrallaExpress.Models;
using MorrallaExpress.Models.DTOs;
using MorrallaExpress.Validations;
using MorrallaExpress.Validations.Rules;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MorrallaExpress.ViewModels
{
    public class ReviewDeliveryViewModel : ViewModelBase
    {
        public DelegateCommand<string> ChangeRatingCommand { get; set; }
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand SaveReviewCommand { get; set; }

        #region Stars

        string _photo;
        public string Photo { get => _photo; set => SetProperty(ref _photo, value); }

        int _rating;
        public int Rating { get => _rating; set => SetProperty(ref _rating, value); }

        string _driverName;
        public string DriverName { get => _driverName; set => SetProperty(ref _driverName, value); }

        bool _pEnabld;
        public bool PickerEnabled { get => _pEnabld; set => SetProperty(ref _pEnabld, value); }

        ValidatableObject<string> reason = new ValidatableObject<string>();
        public ValidatableObject<string> Reason
        {
            get => reason;
            set => SetProperty(ref reason, value);
        }

        ValidatableObject<string> review = new ValidatableObject<string>();
        public ValidatableObject<string> Review
        {
            get => review;
            set => SetProperty(ref review, value);
        }
        OrderModel CurrentOrder;
        UserSession Driver;

        bool _star1;
        public bool Star1 { get => _star1; set => SetProperty(ref _star1, value); }
        float _op1;
        public float Op1 { get => _op1; set => SetProperty(ref _op1, value); }
        bool _star2;
        public bool Star2 { get => _star2; set => SetProperty(ref _star2, value); }
        float _op2;
        public float Op2 { get => _op2; set => SetProperty(ref _op2, value); }
        bool _star3;
        public bool Star3 { get => _star3; set => SetProperty(ref _star3, value); }
        float _op3;
        public float Op3 { get => _op3; set => SetProperty(ref _op3, value); }
        bool _star4;
        public bool Star4 { get => _star4; set => SetProperty(ref _star4, value); }
        float _op4;
        public float Op4 { get => _op4; set => SetProperty(ref _op4, value); }
        bool _star5;
        public bool Star5 { get => _star5; set => SetProperty(ref _star5, value); }
        float _op5;
        public float Op5 { get => _op5; set => SetProperty(ref _op5, value); }
        #endregion
        public ReviewDeliveryViewModel(INavigationService navigationService, IHttpService httpService) 
            : base(navigationService, httpService)
        {
            ChangeRating("3");
            ChangeRatingCommand = new DelegateCommand<string>(ChangeRating);
            GoBackCommand = new DelegateCommand(async () => await NavigateBack(new NavigationParameters { { "model", CurrentOrder } }));
            SaveReviewCommand = new DelegateCommand(async () => await SaveReview());
            Photo = "User_icn.png";
            AddValidations();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters["model"] is OrderModel m)
            {
                CurrentOrder = m;
                Driver = CurrentOrder.Driver;
                DriverName = Driver.Name;
                if (!string.IsNullOrEmpty(Driver.Photo))
                   Photo = Driver.Photo;
            }
        }

        void ChangeRating(string rating)
        {
            switch (rating)
            {
                case "1":
                    Op1 = 3.0f;
                    Op2 = 0.3f;
                    Op3 = 0.3f;
                    break;
                case "2":
                    Op1 = 0.3f;
                    Op2 = 3.0f;
                    Op3 = 0.3f;
                    break;
                case "3":
                    Op1 = 0.3f;
                    Op2 = 0.3f;
                    Op3 = 3.0f;
                    break;
             
            }
        }

        async Task SaveReview()
        {
            if (Validate())
            {
                var res = false;
                using (UserDialogs.Instance.Loading("Cargando..."))
                    res = await HttpService.ReviewOrder(Rating, Reason.Value, Review.Value, CurrentOrder.DeliveryId);
                if (res)
                {
                    var list = HttpService.GetCurrentOrderHistory();
                    CurrentOrder = list.FirstOrDefault(order => order.DeliveryId == CurrentOrder.DeliveryId);
                    await PopUp("¡Gracias por tu calificación!", "Con tu opinión seguimos mejorando.", "Aceptar", GoBackCommand);
                }
                else
                    await PopUp("Error!", "Ocurrió un error al procesar su solicitud, intente de nuevo.", "Aceptar");
            }
        }

        bool Validate()
        {
           // var isValidReason = reason.Validate();
            //Reason.IsValid = isValidReason;

            var isValidReview = review.Validate();
            Review.IsValid = isValidReview;
            return /*isValidReason &&*/ isValidReview;
        }

        void AddValidations()
        {
            reason.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* El campo es requerido" });
            review.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "* El campo es requerido" });
        }
    }
}
