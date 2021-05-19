using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Models.DTOs
{
   public class SettingsModel
    {
        public int SettingId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }

    }
}
