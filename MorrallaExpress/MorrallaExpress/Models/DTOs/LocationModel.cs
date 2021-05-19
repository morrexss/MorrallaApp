using System;
using System.Collections.Generic;
using System.Text;
using MorrallaExpress.Models.DTOs;
using Prism.Commands;

namespace MorrallaExpress.Models
{
    public class LocationModel
    {
        public DelegateCommand<LocationModel> DeleteCommand { get; set; }
            = new DelegateCommand<LocationModel>((o) => { });

        public int LocationId { get; set; }
        public string Name { get; set; }
        public string ApplicationUserId { get; set; }
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ExteriorNumber { get; set; }
        public string InteriorNumber { get; set; }
        public int? SubdivisionId { get; set; }
        public string SubdivisionName { get; set; }
        public int? CityId { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int? StateId { get; set; }
        public string PostalCode { get; set; }
        public string AuthorizedPersonel { get; set; }
        public string Photo { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // App Specific properties
        public string CityandState { get => City + ", " + State;}
        public string FullAddress { get => Address1 + " #" + ExteriorNumber + " "+ SubdivisionName; }
        public AuthorizedPersonnelModel[] AuthorizedPersonnel { get; set; } = new AuthorizedPersonnelModel[] { };
    }
}
