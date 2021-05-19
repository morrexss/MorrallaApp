using System;
using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace MorrallaExpress.Extensions
{
    public static class MapExtensions
    {
        static readonly double EarthRadiusInMeters = 6378137;
        public static List<Position> GenerateCircleCoordinates(Position position, double radius)
        {
            double latitude = position.Latitude.ToRadians();
            double longitude = position.Longitude.ToRadians();
            double distance = radius / EarthRadiusInMeters;
            var positions = new List<Position>();

            for (int angle = 0; angle <= 360; angle++)
            {
                double angleInRadians = ((double)angle).ToRadians();
                double latitudeInRadians = Math.Asin(Math.Sin(latitude) * Math.Cos(distance) + Math.Cos(latitude) * Math.Sin(distance) * Math.Cos(angleInRadians));
                double longitudeInRadians = longitude + Math.Atan2(Math.Sin(angleInRadians) * Math.Sin(distance) * Math.Cos(latitude), Math.Cos(distance) - Math.Sin(latitude) * Math.Sin(latitudeInRadians));

                var pos = new Position(latitudeInRadians.ToDegrees(), longitudeInRadians.ToDegrees());
                positions.Add(pos);
            }

            return positions;
        }

        private static double ToRadians(this double angle) =>
            Math.PI * angle / 180.0;

        private static double ToDegrees(this double angle) =>
            angle * (180.0 / Math.PI);
    }
}
