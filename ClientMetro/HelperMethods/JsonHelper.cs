using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ClientMetro.HelperMethods
{
    public static class JsonHelper
    {
        public static string GetValue(JObject json, string propertyName)
        {
            return json.GetValue(propertyName).Value<string>();
        }
    }
}
