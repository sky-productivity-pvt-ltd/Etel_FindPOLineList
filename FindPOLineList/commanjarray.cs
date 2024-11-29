using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindPOLineList
{
    class commanjarray
    {
        public static JObject CommanJobject(string key, string value)
        {
            JObject jo_main = new JObject
            {
                { "key", key },
                { "value", value }
            };

            return jo_main;
        }
    }
}
