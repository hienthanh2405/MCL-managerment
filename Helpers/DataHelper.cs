using Newtonsoft.Json;
using System.Collections.Generic;

namespace API.Helpers
{
    public static class DataHelper<T>
    {
       public static List<T> ProtectNullJsonParse( string source)
        {
            if(source == null)
            {
                return new List<T>();
            } else
            {
                return JsonConvert.DeserializeObject<List<T>>(source);
            }
        }
    }
}
