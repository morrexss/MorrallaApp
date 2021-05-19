using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Models.DTOs
{
    public class SubdivisionModel
    {
        public int SubdivisionId { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public int PostalCodeId { get; set; }
    }
}
