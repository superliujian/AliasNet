using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
///<summary>
///AliasNet,提供一种简单，方便，封装，统一的GET/POST方法
///Copyright:liujian
///Email:liu_jian_china@qq.com
///Web:http://www.superliujian.net
///<summary>
namespace Net.Superliujian.AliasNet
{
   /// <summary>
   /// JsonClassBase基类，所有继承自此类的子类都可以通过基类提供的方法将JSON字符串转换为类实体
   /// </summary>
   public class JsonClassBase
    {
        /// <summary>
        /// 将JSON字符串转换为类实体
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>类实体</returns>
       public static T FromJson<T>(string json)
       {
           return (T)new JsonSerializer().Deserialize(new JsonReader(new StringReader(json)), typeof(T));
       }
    }
}
