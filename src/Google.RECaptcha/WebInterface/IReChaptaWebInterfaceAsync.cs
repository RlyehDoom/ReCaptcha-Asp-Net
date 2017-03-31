using System.Net;
using System.Threading.Tasks;

namespace Google.RECaptcha.WebInterface
{
    internal interface IReChaptaWebInterfaceAsync
    {
        Task<ReCaptchaJsonResponse> PostUserAnswerAsync(string response, string secretKey);
        Task<ReCaptchaJsonResponse> PostUserAnswerAsync(string response, string secretKey, WebProxy proxy);
    }
}