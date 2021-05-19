using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Models.DTOs
{
    public class StateModel
    {
        public int StateId { get; set; }
        public string Name { get; set; }
        public CityModel[] Cities { get; set; }
    }
}
