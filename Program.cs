using System;
using System.IO;
using System.Net;
using System.Text;

namespace token
{
    class Program
    {
        static void Main()
        {
            string home_id = "YOUR_OWN_HOME_ID";
            string email = "YOUR_NETATMO_LOGIN_EMAIL";
            string password = "YOUR_NETATMO_LOGIN_PASSWORD";
            string tokenValue = string.Empty;
            string netatmoLogin = "https://auth.netatmo.com/en-us/access/login";
            string cevap;
            HttpWebRequest webTalebi = (HttpWebRequest)WebRequest.Create(netatmoLogin);
            webTalebi.CookieContainer = new CookieContainer();
            webTalebi.Method = "GET";
            var httpResponse = (HttpWebResponse)webTalebi.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                cevap = streamReader.ReadToEnd();
                foreach (Cookie cook in httpResponse.Cookies)
                    webTalebi.CookieContainer.Add(cook);
            }
            string[] htmlParca = cevap.Split('"');
            for (int i = 0; i < htmlParca.Length; i++)
            {
                if (htmlParca[i] == "_token")
                    tokenValue = htmlParca[i + 2];
            }
            string _myParameters = "email=" + email + "&password=" + password + "&stay_logged=on&_token=" + tokenValue;
            string postlogin = "https://auth.netatmo.com/access/postlogin";

            HttpWebRequest talep = (HttpWebRequest)WebRequest.Create(postlogin);
            talep.CookieContainer = webTalebi.CookieContainer;
            talep.ContentType = "application/x-www-form-urlencoded";
            talep.Method = "POST";
            talep.KeepAlive = true;
            talep.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            talep.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate, br";
            talep.Headers[HttpRequestHeader.AcceptLanguage] = "tr-TR,tr;q=0.8,en-US;q=0.5,en;q=0.3";
            talep.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
            talep.AllowAutoRedirect = false;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(_myParameters);
            talep.ContentLength = byte1.Length;
            talep.SendChunked = true;
            Stream newStream = talep.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);
            string cookies = "";
            HttpWebResponse _response = (HttpWebResponse)talep.GetResponse();
            foreach (Cookie cook in _response.Cookies)
                cookies += "{" + cook;
            string[] parca = cookies.Split('{');
            string tokenCookie = string.Empty;
            for (int i = 0; i < parca.Length; i++)
            {
                if (parca[i].StartsWith("net"))
                    tokenCookie = parca[i];
            }
            string netatmoToken = tokenCookie.Replace("%7C", "|").Split('=')[1];
            postlogin = "https://app.netatmo.net/api/sethomedata";
            _myParameters = "{\"home\":{\"id\":\"" + home_id + "\",\"therm_heating_priority\":\"comfort\"}}";//for comfort mode
            //_myParameters = "{\"home\":{\"id\":\"" + home_id + "\",\"therm_heating_priority\":\"eco\"}}";//for eco mode
            talep = (HttpWebRequest)WebRequest.Create(postlogin);
            talep.CookieContainer = new CookieContainer();
            talep.ContentType = "application/json";
            talep.Headers[HttpRequestHeader.Authorization] = "Bearer " + netatmoToken;
            talep.Method = "POST";
            talep.KeepAlive = true;
            talep.Accept = "application/json;text/plain;*/*";
            talep.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate, br";
            talep.Headers[HttpRequestHeader.AcceptLanguage] = "tr-TR,tr;q=0.8,en-US;q=0.5,en;q=0.3";
            talep.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";
            talep.AllowAutoRedirect = false;
            encoding = new ASCIIEncoding();
            byte1 = encoding.GetBytes(_myParameters);
            talep.ContentLength = byte1.Length;
            talep.SendChunked = true;
            newStream = talep.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);

            _response = (HttpWebResponse)talep.GetResponse();
            using (var streamReader = new StreamReader(_response.GetResponseStream()))
                Console.WriteLine(streamReader.Read());

        }
    }
}
