using System;
using Prism.Commands;

namespace MorrallaExpress.Models
{
    public class MasterPageItem
    {
        public string Title { get; set; }
        public string NavigateTo { get; set; }
        public FontAwesomeIcon Icon { get; set; }
        public string IconSource { get => $"&#x{Icon.Icon};"; }
        public bool IsAbsolute { get; set; }
        public DelegateCommand NavigateToCommand { get; set; }
    }

    public class FontAwesomeIcon
    {
        public string Icon { get; set; }
        public int FontType { get; set; }
        public string FontStyle
        {
            get
            {
                switch (FontType)
                {
                    case 0: //Regular Font
                        return "FontAwesomeRegular";
                    case 1: //Solid Font
                        return "FontAwesomeSolid";
                    case 2: //Brands Font
                        return "FontAwesomeBrands";
                    default:
                        return "FontAwesomeRegular";
                }
            }
        }
    }
}
