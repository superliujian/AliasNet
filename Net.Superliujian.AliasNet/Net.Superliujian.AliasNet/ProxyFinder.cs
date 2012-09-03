/*
 * Created by SharpDevelop.
 * User: KenLew
 * Date: 2012/3/7 星期三
 * Time: 10:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace Net.Superliujian.Common.Net
{
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
	[Serializable]
	public class ProxyCollection:List<Proxy>
	{
		public static void Serialize(ProxyCollection proxy,Stream stream)
		{
			BinaryFormatter formatter=new BinaryFormatter();
			formatter.Serialize(stream,proxy);
		}
		
		public static ProxyCollection Deserialize(Stream stream)
		{
			BinaryFormatter formatter=new BinaryFormatter();
			return (ProxyCollection)formatter.Deserialize(stream);
		}
	}
	
	/// <summary>
	/// Description of ProxyFinder.
	/// </summary>
	public class ProxyFinder
	{
			//获取代理地址的地方，默认为无忧代理最新代理
			public static string _51proxied_http_fast="http://www.51proxied.com/http_fast.html";
			public static string _51proxied_http_anonymous= "http://www.51proxied.com/http_anonymous.html";
			public static string _51proxied_http_non_anonymous ="http://www.51proxied.com/http_non_anonymous.html";
			public static string _51proxied_socks5 = "http://www.51proxied.com/socks5.html";


			public static List<Proxy> _51proxied_HttpProxy(string proxyUrl)
			{
				return _51proxied_HttpProxy(proxyUrl, "");
			}
			public static List<Proxy> _51proxied_HttpProxy(string proxyUrl,string regionFilter)
			{
				string resultString=new GetAndPostHelper().GETString(proxyUrl);
				HtmlAgilityPack.HtmlDocument doc=new HtmlAgilityPack.HtmlDocument();
				doc.LoadHtml(resultString);
				HtmlNode  divNode=doc.GetElementbyId("tb");
				HtmlNodeCollection tdNodesCollection=divNode.SelectNodes("//div/table//td");
				List<HtmlNode> tdNodes=new List<HtmlNode>();
				foreach(HtmlNode node in tdNodesCollection)
					tdNodes.Add(node);
				List<Proxy> proxy=new List<Proxy>();
				for(int i=0;i<tdNodes.Count/4;i++)
				{
					string region=tdNodes[i*4+3].InnerText.Trim();
					if(string.IsNullOrEmpty(regionFilter) ||region==regionFilter )
					{
						Proxy p=new Proxy();
						p.IP=tdNodes[i*4+1].InnerText.Trim();;
						p.Port=int.Parse(tdNodes[i*4+2].InnerText.Trim());
						p.Region=region;
						proxy.Add(p);
					}
				}
				return proxy;
			}
	}
}
