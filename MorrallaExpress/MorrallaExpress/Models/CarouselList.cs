using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Models
{
    public class CarouselList
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }
        public string Fondo { get; set; }
        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand NextCommand { get; set; }
    }
}
