using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace Net.Superliujian.Common.Net
{
	
	public class Parameters : Dictionary<string, string>
	{
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<string, string> pair in this)
			{
				sb.Append(string.Format("{0}={1}&", pair.Key, pair.Value));
			}
			if (sb.Length > 0)
				sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}
	}
	
	/// <summary>
	/// HTTP通用处理库
	/// 以POST和GET两种方式处理HTTPS连接，并且返回字符串或二进制数据
	/// </summary>
	public class GetAndPostHelper
	{

	   public static string FireFoxAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.2.23) Gecko/20110920 Firefox/3.6.23";
		public static string IE7Agent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; InfoPath.2; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET4.0C; .NET4.0E)";

		//去除HTML标记
		public static string NoHtml(string strHtml)
		{
			string[] re ={"<[\\s]*?script[^>]*?>[\\s\\S]*?<[\\s]*?\\/[\\s]*?script[\\s]*?>",
						  "<[\\s]*?style[^>]*?>[\\s\\S]*?<[\\s]*?\\/[\\s]*?style[\\s]*?>",
						  "<[^>]+>",
						  "\\s+"};

			string noHtml = strHtml;
			for (int i = 0; i < re.Length; i++)
			{
				Regex regex = new Regex(re[i], RegexOptions.IgnoreCase);
				noHtml = regex.Replace(noHtml, "");
			}
			return noHtml.Trim();
		}

		#region Private Method 私有辅助函数

		private bool _IsHttps = false;
		public bool IsHttps { get { return _IsHttps; } set { _IsHttps = value; } }

		private CookieContainer _CookieContainers = null;

		public void ClearCookies()
		{
			_CookieContainers = null;
		}

		public void SetCookies(CookieContainer cookies)
		{
			_CookieContainers = cookies;
		}
		private NetworkCredential _Credential = null;
		public void ClearCredential()
		{
			_Credential = null;
		}

		public void SetCredential(string user, string pwd)
		{
			_Credential = new NetworkCredential(user, pwd);
		}

		private static WebProxy _Proxy = null;
		public void ClearProxy()
		{
			_Proxy = null;
		}

		public void SetProxy(string ip, int port)
		{
			_Proxy = new WebProxy(ip, port);
		}
		#region 构造函数
		public GetAndPostHelper(bool https)
		{
			IsHttps = https;
		}

		public GetAndPostHelper()
		{
			IsHttps = false;
		}
		#endregion
		public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;//信任所有连接
		}

		//SSL信任所有连接 ，在连接前调用这个函数
		public void SSLCertification()
		{
			if(IsHttps)
			System.Net.ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(CheckValidationResult);
		}

		private string ResponseToString(HttpWebResponse response)
		{
			if (response == null)
			{
				return "连接失败:没有得到返回信";
			}
            StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("gb2312"));
			return (reader.ReadToEnd().Trim()) ;
		}


		private byte[] ResponseToBytes(HttpWebResponse response)
		{
			if (response == null)
				return null;
			int bytesRead = 0;
			MemoryStream ms = new MemoryStream();
			Stream stream = response.GetResponseStream();
			do
			{
				byte[] buffer = new byte[256];
				bytesRead = stream.Read(buffer, 0, 256);
				ms.Write(buffer, 0, (bytesRead < 256 ? bytesRead : 256));
			}
			while (bytesRead > 0);
			byte[] data = ms.ToArray();
			ms.Close();
			stream.Close();
			response.Close();
			return data;
		}
		#endregion


		public void DownloadFile(string url, string fileName)
		{
			WebClient wb=new WebClient();
			if (_Credential != null)
			{
				wb.Credentials = _Credential;

			}
			try
			{
				wb.DownloadFile(url, fileName);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			//using (Stream fs=new FileStream(fileName,FileMode.OpenOrCreate,FileAccess.Write,FileShare.Read))
			//{
			//    Stream ns = GETResponse(url).GetResponseStream();
			//    BinaryWriter sw = new BinaryWriter(fs);
			//    BinaryReader sr = new BinaryReader(ns);
			//    byte[] bytes = new byte[ns.;
			//    sr.Read(bytes, 0, bytes.Length);
			//    sw.Write(bytes);
			//    sw.Flush();
			//    sw.Close();
			//    sr.Close();
			//    ns.Close();
			//    fs.Close();
			//}

			
		}


		#region GET
		
		//以GET方式得到响应
		public HttpWebResponse GETResponse(string url)
		{
			SSLCertification();
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method="GET";
			request.ContentType = "application/x-www-form-urlencoded";
			request.UserAgent = IE7Agent;
			request.KeepAlive = true;
			request.AllowAutoRedirect = true;
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
			if(_Proxy!=null)
				request.Proxy = _Proxy;
			if (_Credential != null)
			{
			   
				//method 1
				//CredentialCache cache = new CredentialCache();
				//cache.Add(new Uri(url), "Basic", _Credential);
				//request.Credentials = cache;

				//method 2
				request.PreAuthenticate = true;
				request.Credentials = _Credential;

				//method 3
				//string authInfo = _Credential.UserName + ":" + _Credential.Password;
				//authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
				//request.Headers["Authorization"] = "Basic " + authInfo;

			}
			if (_CookieContainers != null)
				request.CookieContainer = _CookieContainers;
			HttpWebResponse response = null;
			try
			{
				response = (HttpWebResponse)request.GetResponse();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return response;
		}

		//以GET方式从指定URL下载字符串
		public string GETString(string url)
		{
			return ResponseToString(GETResponse(url));
		}
		//以GET方式从指定URL下载字节数据
		public byte[] GETBytes(string url)
		{
			return ResponseToBytes(GETResponse(url));
		}

		#endregion

		#region POST
		//以POST方式得到响应
		public HttpWebResponse POSTResponse(string url, string data)
		{
			SSLCertification();

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType="application/x-www-form-urlencoded";
			request.UserAgent = IE7Agent;
			request.KeepAlive = true;
			request.AllowAutoRedirect = true;
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
			if (_Proxy != null)
				request.Proxy=_Proxy;
			if (_Credential != null)
			{
				//method 1
				//CredentialCache cache = new CredentialCache();
				//cache.Add(new Uri(url), "Basic", _Credential);
				//request.Credentials = cache;

				//method 2
				request.PreAuthenticate = true;
				request.Credentials = _Credential;

				//method 3
				//string authInfo = _Credential.UserName + ":" + _Credential.Password;
				//authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
				//request.Headers["Authorization"] = "Basic " + authInfo;
			}
			if (_CookieContainers != null)
				request.CookieContainer = _CookieContainers;

			byte[] bytesData = Encoding.ASCII.GetBytes((data));
			request.ContentLength = bytesData.Length;
			Stream reqStream = request.GetRequestStream();
			reqStream.Write(bytesData, 0, bytesData.Length);
			reqStream.Flush();
			reqStream.Close();
			HttpWebResponse response = null;
			try
			{
				response = (HttpWebResponse)request.GetResponse();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return response;
		}

		//以POST方式得到字节数据
		public byte[] POSTBytes(string url, string stringData)
		{
			return ResponseToBytes(POSTResponse(url, stringData));
		}

		//以POST方式得到字符串
		public string POSTString(string url, string stringData)
		{
			return ResponseToString(POSTResponse(url, stringData));
		}
		#endregion
	}

}
