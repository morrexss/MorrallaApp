using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MorrallaExpress.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object source) =>
            JsonConvert.SerializeObject(source);
        public static T ToObject<T>(this string source) where T : class =>
            JsonConvert.DeserializeObject<T>(source);
    }
}
