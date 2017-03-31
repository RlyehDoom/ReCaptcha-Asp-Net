using System.Linq;
using System.Reflection;

namespace Google.RECaptcha.Internazionalization
{
    public static class LanguageAttributeHelper
    {
        public static string GetLanguage(this ReCaptchaLanguage language)
        {
            var attribute = language.GetType().GetMember(language.ToString()).Select(m => m.GetCustomAttribute<LanguageAttribute>()).FirstOrDefault() ?? new LanguageAttribute(string.Empty);
            return attribute.Value;
        }

        private static T GetCustomAttribute<T>(this MemberInfo type)
        {
            return (T)type.GetCustomAttributes(typeof (T), true).FirstOrDefault();
        }
    }
}