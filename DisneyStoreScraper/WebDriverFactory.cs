using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DisneyStoreScraper
{
    public static class WebDriverFactory
    {
        public static IWebDriver NewWebDriver()
        {
            var service = ChromeDriverService.CreateDefaultService();
            service.EnableVerboseLogging = false;
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;

            // use these options to disable logging
            var options = new ChromeOptions();
            options.AddArgument("--disable-logging");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--log-level=3");

            // can add argument below to run headless
            //options.AddArgument("--headless");

            return new ChromeDriver(service, options);
        }
    }
}
