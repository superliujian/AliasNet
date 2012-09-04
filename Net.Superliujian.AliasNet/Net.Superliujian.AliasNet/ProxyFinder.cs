
using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

///<summary>
///AliasNet,提供一种简单，方便，封装，统一的GET/POST方法
///Copyright:liujian
///Email:liu_jian_china@qq.com
///Web:http://www.superliujian.net
///<summary>
///

namespace Net.Superliujian.AliasNet
{
    /// <summary>
    /// 代理
    /// </summary>
    [Serializable]
    public class Proxy
    {
        public string IP;
        public int Port = 80;
        public string Region;

        public override string ToString()
        {
            return string.Format("{0}:{1}", IP, Port);
        }
    }
    /// <summary>
    /// 代理集合
    /// </summary>
    [Serializable]
    public class ProxyCollection : List<Proxy>
    {
        public static void Serialize(ProxyCollection proxy, Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, proxy);
        }

        public static ProxyCollection Deserialize(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return (ProxyCollection)formatter.Deserialize(stream);
        }
    }

    /// <summary>
    /// 通过ProxyFinder可以方便的从51Proxy上得到各种当前最新的可用的代理
    /// </summary>
    public class ProxyFinder
    {
        //获取代理地址的地方，默认为无忧代理最新代理
        //无忧快速HTTP代理URL
        public static string _51proxied_http_fast = "http://www.51proxied.com/http_fast.html";
        //无忧匿名HTTP代理URL
        public static string _51proxied_http_anonymous = "http://www.51proxied.com/http_anonymous.html";
        //无忧非匿名HTTP代理URL
        public static string _51proxied_http_non_anonymous = "http://www.51proxied.com/http_non_anonymous.html";
        //无忧Socket5代理URL
        public static string _51proxied_socks5 = "http://www.51proxied.com/socks5.html";


        public static List<Proxy> _51proxied_HttpProxy(string proxyUrl)
        {
            return _51proxied_HttpProxy(proxyUrl, "");
        }

        /// <summary>
        /// 从无忧代理网站得到当前最新的代理
        /// </summary>
        /// <param name="proxyUrl">代理Web URL</param>
        /// <param name="regionFilter">过滤指定的region，不过滤使用""或null</param>
        /// <returns>代理命令</returns>
        public static List<Proxy> _51proxied_HttpProxy(string proxyUrl, string regionFilter)
        {
            string resultString = new GetAndPostHelper().GETString(proxyUrl);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(resultString);
            HtmlNode divNode = doc.GetElementbyId("tb");
            HtmlNodeCollection tdNodesCollection = divNode.SelectNodes("//div/table//td");
            List<HtmlNode> tdNodes = new List<HtmlNode>();
            foreach (HtmlNode node in tdNodesCollection)
                tdNodes.Add(node);
            List<Proxy> proxy = new List<Proxy>();
            for (int i = 0; i < tdNodes.Count / 4; i++)
            {
                string region = tdNodes[i * 4 + 3].InnerText.Trim();
                if (string.IsNullOrEmpty(regionFilter) || region == regionFilter)
                {
                    Proxy p = new Proxy();
                    p.IP = tdNodes[i * 4 + 1].InnerText.Trim(); ;
                    p.Port = int.Parse(tdNodes[i * 4 + 2].InnerText.Trim());
                    p.Region = region;
                    proxy.Add(p);
                }
            }
            return proxy;
        }
    }
}
