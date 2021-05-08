using Newtonsoft.Json;

namespace API.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object value, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(value, formatting);
        }
    }
}
