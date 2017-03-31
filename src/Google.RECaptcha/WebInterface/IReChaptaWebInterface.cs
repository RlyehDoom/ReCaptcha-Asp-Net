using System.Net;

namespace Google.RECaptcha.WebInterface
{
    internal interface IReChaptaWebInterface
    {
        ReCaptchaJsonResponse PostUserAnswer(string response, string secretKey);
        ReCaptchaJsonResponse PostUserAnswer(string response, string secretKey, WebProxy proxy);
    }
}