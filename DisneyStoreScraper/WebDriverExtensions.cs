using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace DisneyStoreScraper
{
    internal static class WebDriverExtensions
    {
        /// <summary>
        /// Wait to see if an IWebElement is found and return true if it is displayed or false if it is not. Returns the IWebElement if it is found or null if not found.
        /// </summary>
        /// <returns> 
        /// Returns IsFound boolean based on if IWebElement is found with the Func provided.
        /// Returns IWebElement found with the Func provided or null if no element found with the Func provided.
        /// </returns>
        public static (bool IsFound, IWebElement? WebElement) WaitToGetWebElement(this IWebDriver driver, TimeSpan timeout, Func<IWebDriver, IWebElement> func)
        {
            IWebElement? webElement = null;
            bool isFound = false;

            try
            {
                var wait = new WebDriverWait(driver, timeout);

                isFound = wait.Until(_ =>
                {
                    try
                    {
                        webElement = func.Invoke(driver);

                        var isDisplayed = webElement.Displayed;

                        return isDisplayed;
                    }
                    catch(Exception)
                    {
                        return false;
                    }
                });
            }
            catch (WebDriverTimeoutException)
            {
                isFound = false;
            }

            return (isFound, webElement);
        }

        /// <summary>
        /// Wait to see if an IWebElement is found and return true if able to find and click the web element. Returns the IWebElement if it is found.
        /// </summary>
        /// <returns>IsClicked will be true if the IWebElement is found and clicked. IsClicked will be false if the element is not found or cannot be clicked. WebElement will be null if not found. </returns>
        public static (bool IsClicked, IWebElement? WebElement) WaitToClickButton(this IWebDriver driver, TimeSpan timeout, Func<IWebDriver, IWebElement> func)
        {
            var (isFound, element) = driver.WaitToGetWebElement(timeout, func);

            if (!isFound)
            {
                return (isFound, element);
            }

            try
            {
                new Actions(driver).Click(element).Perform();
                return (isFound, element);
            }
            catch (Exception)
            {
                return (false, element);
            }
        }
    }
}
