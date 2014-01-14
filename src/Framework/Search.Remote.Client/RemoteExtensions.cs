using N2.Engine;
using N2.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Security;

namespace N2.Search.Remote.Client
{
    public class RemoteExtensions
    {
        public static string RequestJson(string httpMethod, string url, string requestBody, int timeout = 15000, string sharedSecret = null)
        {
            HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(url);
            hwr.Method = httpMethod;
            hwr.ContentType = "application/json";
            hwr.UserAgent = "N2 Remote Search Client 1.0";
            hwr.Timeout = timeout;

            if (!string.IsNullOrEmpty(sharedSecret))
            {
                hwr.CookieContainer = new CookieContainer();
                var cookie = new Cookie("Secret", sharedSecret, "/", url.ToUrl().Domain);
                hwr.CookieContainer.Add(cookie);
            }
            if (string.IsNullOrEmpty(requestBody))
                hwr.ContentLength = 0;
            else
            {
                using (var s = hwr.GetRequestStream())
                using (var tw = new StreamWriter(s))
                {
                    tw.Write(requestBody);
                }
            }

            using (var wr = hwr.GetResponse())
            {
                if (wr.ContentLength == 0)
                    return "";

                using (var s = wr.GetResponseStream())
                using (var sr = new StreamReader(s))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
