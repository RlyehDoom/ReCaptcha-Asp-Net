using Google.RECaptcha.Exceptions;
using Google.RECaptcha.Internazionalization;
using Google.RECaptcha.WebInterface;
using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Google.RECaptcha
{
    internal class ReCaptchaObject
    {
        private string _captchaDiv, _secretKey, _language;
        private bool _configured;
        
        internal ReCaptchaObject()
        {
            // Auto .config configuration
            var reader = new AppSettingsReader();
            try
            {
                string secretKey = reader.GetValue("recaptcha-secret-key", typeof(string)).ToString();
                string publicKey = reader.GetValue("recaptcha-public-key", typeof(string)).ToString();

                Initialize(publicKey, secretKey);
            }
            catch
            {
                // No configuration on .config
            }
            try
            {
                _language = reader.GetValue("recaptcha-language-key", typeof(string)).ToString();
            }
            catch
            {
                // No language on .config
            }
        }

        internal ReCaptchaObject(string publicKey, string secretKey, ReCaptchaLanguage? defaultLanguage = null)
        {
            Initialize(publicKey, secretKey, defaultLanguage);
        }

        private void Initialize(string publicKey, string secretKey, ReCaptchaLanguage? defaultLanguage = null)
        {
            if (string.IsNullOrWhiteSpace(publicKey))
            {
                throw new ArgumentNullException("publicKey");
            }
            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new ArgumentNullException("secretKey");
            }
            if (defaultLanguage.HasValue)
            {
                _language = defaultLanguage.Value.GetLanguage();
            }
            _configured = true;
            _secretKey = secretKey;
            _captchaDiv = string.Format("<div class='g-recaptcha' data-sitekey='{0}'></div><script src='https://www.google.com/recaptcha/api.js{{0}}'></script>", publicKey);
        }

        private string GetHlCode(ReCaptchaLanguage? language)
        {
            string strLang = language.HasValue ? language.Value.GetLanguage() : _language;
            return string.IsNullOrWhiteSpace(strLang) ? "" : "?hl=" + strLang;
        }

        private void CheckIfIamConfigured()
        {
            if (_configured) { return; }
            throw new ReCaptchaException("ReCaptcha is not configured. Get your site and secret keys from google. And call function ReCaptcha.Configure(publicKey, secretKey), or add the keys to the .config file <add key='recaptcha-public-key' value='...' /><add key='recaptcha-site-key' value='...'/>");
        }

        internal IHtmlString GetCaptcha(ReCaptchaLanguage? language)
        {
            CheckIfIamConfigured();
            return new HtmlString(string.Format(_captchaDiv, GetHlCode(language)));
        }

        internal bool ValidateResponse(IReChaptaWebInterface webInterface, string response)
        {
            CheckIfIamConfigured();
            var answer = webInterface.PostUserAnswer(response, _secretKey);
            TreatReCaptchaError(answer);
            return answer.Success;
        }

        internal bool ValidateResponse(IReChaptaWebInterface webInterface, string response, WebProxy proxy)
        {
            CheckIfIamConfigured();
            var answer = webInterface.PostUserAnswer(response, _secretKey, proxy);
            TreatReCaptchaError(answer);
            return answer.Success;
        }

        internal async Task<bool> ValidateResponseAsync(IReChaptaWebInterfaceAsync webInterface, string response)
        {
            CheckIfIamConfigured();
            var answer = await webInterface.PostUserAnswerAsync(response, _secretKey);
            TreatReCaptchaError(answer);
            return answer.Success;
        }

        internal async Task<bool> ValidateResponseAsync(IReChaptaWebInterfaceAsync webInterface, string response, WebProxy proxy)
        {
            CheckIfIamConfigured();
            var answer = await webInterface.PostUserAnswerAsync(response, _secretKey, proxy);
            TreatReCaptchaError(answer);
            return answer.Success;
        }

        private static void TreatReCaptchaError(ReCaptchaJsonResponse answer)
        {
            var error = new ReCaptchaError(answer.ErrorCodes);

            if (error.InvalidInputSecret)
            {
                throw new ReCaptchaException("Invalid ReCaptcha Secret Key !");
            }
            if (error.InvalidInputResponse)
            {
                throw new ReCaptchaException("Invalid Input Response, make sure you are passing correctly the user answer from the Captcha.");
            }
        }
    }
}