using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class JsonUtils
    {
        public static T Convert<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, new LocalDateConverter());
        }
        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

    }
    public class LocalDateConverter : Newtonsoft.Json.Converters.IsoDateTimeConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (typeof(Nullable<DateTime>) == objectType)
            {
                if (reader.Value == null) return null;
                string str = reader.Value.ToString();
                if (string.IsNullOrWhiteSpace(str) || string.Equals(str, "null"))
                {
                    return null;
                }
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
            else
            {
                DateTime dtime;
                if (DateTime.TryParse(reader.Value.ToString(), out dtime))
                {
                    return dtime;
                }
                else
                {
                    dtime = DateTime.Parse("1901-01-01");
                }
                return dtime;
            }

        }
    }
}
