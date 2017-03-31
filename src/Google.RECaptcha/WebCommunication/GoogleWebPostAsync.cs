using Google.RECaptcha.WebInterface;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Google.RECaptcha.WebCommunication
{
#if !NET40
    internal class GoogleWebPostAsync : GoogleBaseWebPost, IReChaptaWebInterfaceAsync
    {
        public async Task<ReCaptchaJsonResponse> PostUserAnswerAsync(string response, string secretKey)
        {
            return await PostUserAnswerAsync(response, secretKey, null);
        }

        public async Task<ReCaptchaJsonResponse> PostUserAnswerAsync(string response, string secretKey, WebProxy proxy)
        {
            string postData = GetPostData(response, secretKey);
            var webRequest = CreatePostWebRequestAsync(postData, proxy);
            return await GetAnswerAsync(await webRequest);
        }

        private async Task<WebRequest> CreatePostWebRequestAsync(string postData, WebProxy proxy)
        {
            var webRequest = CreateEmptyPostWebRequest(proxy);
            webRequest.ContentLength = postData.Length;

            using (var requestWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                await requestWriter.WriteAsync(postData);
            }
            return webRequest;
        }

        private async Task<ReCaptchaJsonResponse> GetAnswerAsync(WebRequest webRequest)
        {
            var webResponse = webRequest.GetResponseAsync();
            return JsonConvert.DeserializeObject<ReCaptchaJsonResponse>(await ReadAnswerFromWebResponseAsync(webResponse));
        }

        private async Task<string> ReadAnswerFromWebResponseAsync(Task<WebResponse> webResponse)
        {
            Stream responseStream = (await webResponse).GetResponseStream();

            if (responseStream == null)
            {
                throw new HttpException(string.Format("No answer from {0}. Check the server web condition.", GoogleRecapthcaUrl));
            }

            using (var responseReader = new StreamReader(responseStream))
            {
                string answer = await responseReader.ReadToEndAsync();
                return answer;
            }
        }
    }
#endif
}