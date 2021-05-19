using System;
using Xamarin.Forms.Maps;

namespace MorrallaExpress.Controls
{
    public class CircledMap : Map
    {
        public CustomCircle Circle { get; set; }
    }

    public class CustomCircle
    {
        public Position Position { get; set; }
        public double Radius { get; set; }
    }
}
