using Google.RECaptcha.WebInterface;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Web;

namespace Google.RECaptcha.WebCommunication
{
    internal class GoogleWebPost : GoogleBaseWebPost, IReChaptaWebInterface
    {
        public ReCaptchaJsonResponse PostUserAnswer(string response, string secretKey)
        {
            return PostUserAnswer(response, secretKey, null);
        }

        public ReCaptchaJsonResponse PostUserAnswer(string response, string secretKey, WebProxy proxy)
        {
            string postData = GetPostData(response, secretKey);
            var webRequest = CreatePostWebRequest(postData, proxy);
            return GetAnswer(webRequest);
        }

        private WebRequest CreatePostWebRequest(string postData, WebProxy proxy)
        {
            var webRequest = CreateEmptyPostWebRequest(proxy);
            webRequest.ContentLength = postData.Length;

            using (var requestWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                requestWriter.Write(postData);
            }
            return webRequest;
        }

        private ReCaptchaJsonResponse GetAnswer(WebRequest webRequest)
        {
            var webResponse = webRequest.GetResponse();
            return JsonConvert.DeserializeObject<ReCaptchaJsonResponse>(ReadAnswerFromWebResponse(webResponse));
        }

        private string ReadAnswerFromWebResponse(WebResponse webResponse)
        {
            Stream responseStream = webResponse.GetResponseStream();

            if (responseStream == null)
            {
                throw new HttpException(string.Format("No answer from {0}. Check the server web condition.", GoogleRecapthcaUrl));
            }

            using (var responseReader = new StreamReader(responseStream))
            {
                string answer = responseReader.ReadToEnd();
                return answer;
            }
        }
    }
}