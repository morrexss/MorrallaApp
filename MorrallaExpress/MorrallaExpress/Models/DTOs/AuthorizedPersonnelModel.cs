using System;
namespace MorrallaExpress.Models.DTOs
{
    public class AuthorizedPersonnelModel
    {
        public int AuthorizedPersonelId { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public int LocationId { get; set; }
        public string Location { get; set; }
    }
}
