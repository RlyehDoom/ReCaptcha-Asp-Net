using System;

namespace Google.RECaptcha.Internazionalization
{
    internal class LanguageAttribute : Attribute
    {
        internal string Value { get; private set; }

        internal LanguageAttribute(string value)
        {
            Value = value;
        }
    }
}