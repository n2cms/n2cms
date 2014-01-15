using System;
using System.Net;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Reimers.Google
{
    public delegate void CallbackDelegate<T>(T item);

    public class TokenRequestor
    {
        #region Fields

        private static readonly string authUrlFormat = "accountType=GOOGLE&Email={0}&Passwd={1}&source=reimers.dk-analyticsreader-0.1&service=analytics";
        private static CultureInfo ci = CultureInfo.GetCultureInfo("en-US");
        private string _sidToken = string.Empty;
        private string _authToken = string.Empty;
        private string _lsidToken = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private int _asyncWaitDelay = 1000;

        #endregion

        #region Constructor

        public TokenRequestor() { }

        public TokenRequestor(string username, string password)
            : this()
        {
            _username = username;
            _password = password;
        }

        #endregion

        #region Properties

        public string Username
        {
            get
            {
                return _username;
            }

            set
            {
                if (!string.Equals(_username, value))
                {
                    _username = value;
                    ClearTokens();
                }
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                if (!string.Equals(_password, value))
                {
                    _password = value;
                    ClearTokens();
                }
            }
        }

        #endregion

        #region Methods

        private void ClearTokens()
        {
            if (Monitor.TryEnter(_sidToken, _asyncWaitDelay) && Monitor.TryEnter(_lsidToken, _asyncWaitDelay) && Monitor.TryEnter(_authToken, _asyncWaitDelay))
            {
                _sidToken = _lsidToken = _authToken = string.Empty;
                Monitor.Exit(_sidToken);
                Monitor.Exit(_lsidToken);
                Monitor.Exit(_authToken);
            }
        }

        private void GetTokens()
        {
            string u = string.Empty;
            string p = string.Empty;
            if (Monitor.TryEnter(_username, _asyncWaitDelay) && Monitor.TryEnter(_password, _asyncWaitDelay))
            {
                if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
                {
                    throw new ArgumentNullException("Username, Password", "Username and/or password not set");
                }
                u = _username;
                p = _password;
                Monitor.Exit(_username);
                Monitor.Exit(_password);
            }

            string authBody = string.Format(authUrlFormat, u, p);
            var req = (HttpWebRequest)WebRequest.Create("https://www.google.com/accounts/ClientLogin");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.UserAgent = "Reimers.dk Token Requestor";
            Stream stream = req.GetRequestStream();
            var sw = new StreamWriter(stream);
            sw.Write(authBody);
            sw.Close();
            sw.Dispose();
            var response = (HttpWebResponse)req.GetResponse();
            var sr = new StreamReader(response.GetResponseStream());
            var token = sr.ReadToEnd();
            sr.Close();
            var tokens = token.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in tokens)
            {
                if (item.StartsWith("Auth="))
                {
                    _authToken = item.Replace("Auth=", string.Empty);
                }
                else if (item.StartsWith("SID="))
                {
                    _sidToken = item.Replace("SID=", string.Empty);
                }
                else if (item.StartsWith("LSID="))
                {
                    _lsidToken = item.Replace("LSID=", string.Empty);
                }
            }
        }

        public string GetSidToken()
        {
            if (string.IsNullOrEmpty(_sidToken))
            {
                GetTokens();
            }

            return _sidToken;
        }

        public void GetSidTokenAsync(CallbackDelegate<string> callback)
        {
            if (callback != null)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    string sid = GetSidToken();
                    callback(sid);
                });
            }
        }

        public string GetLsidToken()
        {
            if (string.IsNullOrEmpty(_lsidToken))
            {
                GetTokens();
            }

            return _lsidToken;
        }
        
        public void GetLsidTokenAsync(CallbackDelegate<string> callback)
        {
            if (callback != null)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    string lsid = GetLsidToken();
                    callback(lsid);
                });
            }
        }

        public string GetAuthToken()
        {
            if (string.IsNullOrEmpty(_authToken))
            {
                GetTokens();
            }
            
            return _authToken;
        }

        public void GetAuthTokenAsync(CallbackDelegate<string> callback)
        {
            if (callback != null)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    string auth = GetAuthToken();
                    callback(auth);
                });
            }
        }

        #endregion
    }
}
