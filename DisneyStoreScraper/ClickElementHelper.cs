using OpenQA.Selenium;

namespace DisneyStoreScraper
{
    public static class ClickElementHelper
    {
        public static (bool IsClicked, IWebElement? Element) TryToClickWithConsoleLogs(IWebDriver driver, string nameOfElementToClick, TimeSpan timeout, Func<IWebDriver, IWebElement> func)
        {
            Console.WriteLine($"\nAttempting To Click Element: {nameOfElementToClick}");

            var (isClicked, element) = driver.WaitToClickElement(timeout, func);

            if(isClicked)
            {
                Console.WriteLine($"Clicked Element: {nameOfElementToClick}");
            }
            else
            {
                var message = element is null ? $"Did Not Find Element: {nameOfElementToClick}" : $"Unable To Click Element: {nameOfElementToClick}";
                Console.WriteLine(message);
            }

            return (isClicked, element);
        }
    }
}
