using System.Net;
using System.Web;

namespace Google.RECaptcha.WebCommunication
{
    internal class GoogleBaseWebPost
    {
        protected const string GoogleRecapthcaUrl = "https://www.google.com/recaptcha/api/siteverify";

        protected string GetPostData(string response, string secretKey)
        {
            // For testing purpouses, this shouldn't happened
            if (HttpContext.Current == null) return string.Format("secret={0}&response={1}", secretKey, response);

            string clientIp = GetClientIp();
            return string.Format("secret={0}&response={1}&remoteip={2}", secretKey, response, clientIp);
        }

        private string GetClientIp()
        {
            // Look for a proxy address first
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            // If there is no proxy, get the standard remote address
            if (!string.IsNullOrWhiteSpace(ip) && ip.ToLower() != "unknown")
            {
                return ip;
            }
            return HttpContext.Current.Request.UserHostAddress;
        }

        protected WebRequest CreateEmptyPostWebRequest(WebProxy proxy)
        {
            var webRequest = WebRequest.Create(GoogleRecapthcaUrl);

            if (proxy != null)
            {
                webRequest.Proxy = proxy;
            }

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            return webRequest;
        }
    }
}