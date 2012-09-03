using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Net.Superliujian.Common.Net
{
   public class JsonClassBase
    {
       public static T FromJson<T>(string json)
       {
           return (T)new JsonSerializer().Deserialize(new JsonReader(new StringReader(json)), typeof(T));
       }
    }
}
