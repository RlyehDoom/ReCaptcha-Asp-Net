using Google.RECaptcha.Exceptions;
using NUnit.Framework;
using System;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Web;

namespace Google.RECaptcha.unittest
{
    [TestFixture]
    public class UnitTests : TimedTests
    {
        private string SiteKey = ConfigurationManager.AppSettings["recaptcha-public-key"].ToString();
        private string SecretKey = ConfigurationManager.AppSettings["recaptcha-secret-key"].ToString();

        private const string TestProxyIp = "185.46.151.26"; private const int PortProxy = 8080;/// Working on 04/02/2016 http://www.freeproxylists.net/185.46.151.26.html

        [SetUp]
        public void ResetTest()
        {
            ReCaptcha.ResetConfiguration();
        }

        [Test]
        public void AssertTestWillConectAndFailInvalidUserAnswer()
        {
            ReCaptcha.Configure(SiteKey, SecretKey);
            Assert.Throws<ReCaptchaException>(() => ReCaptcha.ValidateCaptcha("resposta-fajuta"));
        }

        [Test]
        public void AssertTestWillConectAndFailInvalidUserAnswerWithProxy()
        {
            ReCaptcha.Configure(SiteKey, SecretKey);
            Assert.Throws<ReCaptchaException>(() => ReCaptcha.ValidateCaptcha("resposta-fajuta", new WebProxy(TestProxyIp, PortProxy)));
        }

        [Test]
        public void AssertTestWillConectAndFailInvalidUserAnswerAsync()
        {
            Assert.Throws<ReCaptchaException>(() =>
            {  
                try
                {
                    ReCaptcha.Configure(SiteKey, SecretKey);
                    var task = ReCaptcha.ValidateCaptchaAsync("resposta-fajuta");

                    while (task.IsCompleted == false)
                    {
                        Thread.Sleep(1);
                    }

                    var answer = task.Result;
                    Assert.IsFalse(answer);
                }
                catch (AggregateException e)
                {
                    throw e.InnerException;
                }
            });
        }

        [Test]
        public void AssertTestWillConectAndFailInvalidUserAnswerAsyncWithProxy()
        {
            Assert.Throws<ReCaptchaException>(() =>
            {
                try
                {
                    ReCaptcha.Configure(SiteKey, SecretKey);
                    var task = ReCaptcha.ValidateCaptchaAsync("resposta-fajuta", new WebProxy(TestProxyIp, PortProxy));

                    while (task.IsCompleted == false)
                    {
                        Thread.Sleep(1);
                    }

                    var answer = task.Result;
                    Assert.IsFalse(answer);
                }
                catch (AggregateException e)
                {
                    throw e.InnerException;
                }
            });
        }

        [Test]
        public void ExceptionWhenNotConfigured()
        {
            Assert.Throws<ReCaptchaException>(() => ReCaptcha.GetCaptcha());
        }

        [Test]
        public void InvalidSecretKeyException()
        {
            Assert.Throws<ReCaptchaException>(() =>
            {
                ReCaptcha.Configure("something", "Invalid-Secret-Key");
                bool answer = ReCaptcha.ValidateCaptcha("resposta-fajuta");
            });
        }

        [Test]
        public void WrongSiteKeyArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => ReCaptcha.Configure("", "something"));
        }

        [Test]
        public void WrongSecretKeyArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => ReCaptcha.Configure("something", null));
        }

        [Test]
        public void AssertScriptDivIsCorrect()
        {
            ReCaptcha.Configure("my-public-key", "my-secret-key");
            IHtmlString captcha = ReCaptcha.GetCaptcha();
            string captchaString = captcha.ToHtmlString();
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key'></div><script src='https://www.google.com/recaptcha/api.js'></script>", captchaString);
        }

        [Test]
        public void AssertScriptDivIsCorrectWithLanguage()
        {
            ReCaptcha.Configure("my-public-key", "my-secret-key", ReCaptchaLanguage.German);
            IHtmlString captcha = ReCaptcha.GetCaptcha();
            string captchaString = captcha.ToHtmlString();
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key'></div><script src='https://www.google.com/recaptcha/api.js?hl=de'></script>", captchaString);
        }

        [Test]
        public void AssertScriptDivIsCorrectWithLanguageOverrideConfiguration()
        {
            ReCaptcha.Configure("my-public-key", "my-secret-key", ReCaptchaLanguage.EnglishUs);
            IHtmlString captcha = ReCaptcha.GetCaptcha(ReCaptchaLanguage.PortugueseBrazil);
            string captchaString = captcha.ToHtmlString();
            Assert.AreEqual("<div class='g-recaptcha' data-sitekey='my-public-key'></div><script src='https://www.google.com/recaptcha/api.js?hl=pt-BR'></script>", captchaString);
        }
    }
}