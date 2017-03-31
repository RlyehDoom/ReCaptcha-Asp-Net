using System.Net;
using System.Threading.Tasks;
using System.Web;
using Google.RECaptcha.WebCommunication;

namespace Google.RECaptcha
{
    public static class ReCaptcha
    {
        private static ReCaptchaObject _reCaptcha = new ReCaptchaObject();

        public static void Configure(string publicKey, string secretKey, ReCaptchaLanguage? defaultLanguage = null)
        {
            _reCaptcha = new ReCaptchaObject(publicKey, secretKey, defaultLanguage);
        }

        public static void ResetConfiguration()
        {
            _reCaptcha = new ReCaptchaObject();
        }

        public static IHtmlString GetCaptcha(ReCaptchaLanguage? language = null)
        {
            return _reCaptcha.GetCaptcha(language);
        }

        public static bool ValidateCaptcha(string response, WebProxy proxy = null)
        {
            return _reCaptcha.ValidateResponse(new GoogleWebPost(), response, proxy);
        }
        
        public static async Task<bool> ValidateCaptchaAsync(string response, WebProxy proxy = null)
        {
            return await _reCaptcha.ValidateResponseAsync(new GoogleWebPostAsync(), response, proxy);
        }
    }
}