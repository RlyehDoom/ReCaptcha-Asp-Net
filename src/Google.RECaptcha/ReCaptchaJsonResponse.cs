using Newtonsoft.Json;

namespace Google.RECaptcha
{
    internal class ReCaptchaJsonResponse
    {
        [JsonProperty("success")]
        internal bool Success { get; set; }
        [JsonProperty("error-codes")]
        internal string[] ErrorCodes { get; set; }
    }
}