using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace DisneyStoreScraper
{
    public class DisneyScraper
    {
        private readonly IWebDriver _driver;

        public DisneyScraper(IWebDriver driver) => _driver = driver;

        /// <summary>
        /// Looks for popoup using the xPath provided and will stop trying after secondsToWaitForPopupToDisplay is surpassed before popup displays.
        /// </summary>
        /// <param name="xPathForPopup"></param>
        /// <param name="secondToWaitForPopupToDisplay"></param>
        public void ClosePopupIfDisplayed(string xPathForPopup, int secondsToWaitForPopupToDisplay = 10)
        {
            Console.WriteLine("Waiting to see if popup is found.");

            var (popupFound, popupIFrame) = _driver.WaitToGetWebElement(TimeSpan.FromSeconds(secondsToWaitForPopupToDisplay), _ => _.FindElement(By.XPath(xPathForPopup)));
            Console.WriteLine($"Popup found: {popupFound}");

            var xPathForClosePopupButton = "//button[@id='closeIconContainer']";

            if (popupFound)
            {
                try
                {
                    Console.WriteLine("Switching to iFrame that contains popup.");
                    _driver.SwitchTo().Frame(popupIFrame);

                    Console.WriteLine("Attempting to close popup.");

                    var buttonToClosePopUp = _driver.FindElement(By.XPath(xPathForClosePopupButton));

                    new Actions(_driver).Click(buttonToClosePopUp).Perform();
                    Console.WriteLine("Popup Closed.");

                    Console.WriteLine("Switching back to default content.");
                    _driver.SwitchTo().DefaultContent();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error closing popup: {e.GetType()}");
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Searches store for text - returns true if search text box is found and was able to search for text, else returns false if the search text box could not be found.
        /// </summary>
        /// <param name="text"></param>
        public bool SearchStoreForText(string textToSearchFor)
        {
            try
            {
                Console.WriteLine("Looking for search box.");

                // click search button
                var searchBoxElement = _driver.FindElement(By.XPath("//section[@class='siteSearch']/button"));

                Console.WriteLine("Clicking search box.");
                new Actions(_driver).Click(searchBoxElement).Perform();

                Console.WriteLine("Looking for form input to insert textToSearchFor.");
                var (searchTextBoxFound, searchTextBoxElement) = _driver.WaitToGetWebElement(TimeSpan.FromSeconds(2), _ => _.FindElement(By.XPath("//form[@role='search']/div/input[@type='search']")));

                if (searchTextBoxFound)
                {
                    Console.WriteLine("Clicking field to enter text into search box.");
                    new Actions(_driver).Click(searchTextBoxElement).Perform();

                    Console.WriteLine("Entering text to search for into form input.");
                    searchTextBoxElement!.SendKeys(textToSearchFor);

                    // click Search button
                    Console.WriteLine("Looking for search button");

                    var (searchButtonFound, searchButtonElement) = _driver.WaitToGetWebElement(TimeSpan.FromSeconds(2), _ => _.FindElement(By.XPath("//form[@role='search']/div/button")));

                    if (searchButtonFound)
                    {
                        Console.WriteLine("Clicking button to search");
                        new Actions(_driver).Click(searchButtonElement).Perform();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Could not find search button to submit text to search for.");
                    }
                }
                else
                {
                    Console.WriteLine("Could not find text box to enter text to search for.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception Thrown When Attempting to Search For Text: {textToSearchFor}");
                Console.WriteLine($"Exception Type: {e.GetType()}");
                Console.WriteLine($"Exception Message: {e.Message}");
            }
            
            return false;
        }

        /// <summary>
        /// Filters results from Store after searching for text. Returns true if was able to filter results else returns false.
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public bool FilterSearchResultsByProductType(string productType)
        {
            try
            {
                var xPathForFilterButton = "//div[@class='search__filter_sort']/button[@name='search__filter_btn']";

                Console.WriteLine("Searching For Filter Button.");
                var (filterButtonFound, filterButtonElement) = _driver.WaitToGetWebElement(TimeSpan.FromSeconds(2), _ => _.FindElement(By.XPath(xPathForFilterButton)));
                Console.WriteLine($"Filter Button Found: {filterButtonFound}");

                if (filterButtonFound)
                {
                    Console.WriteLine("Clicking Filter Button.");
                    new Actions(_driver).Click(filterButtonElement).Perform();

                    var xPathForProductTypeFilterButton = "//div[@class='refinements']/div/header/button[@title='Product Type']";

                    Console.WriteLine("Searching For 'Product Type' Filter Button");
                    var (productTypeButtonFound, productTypeButtonElement) = _driver.WaitToGetWebElement(TimeSpan.FromSeconds(2), _ => _.FindElement(By.XPath(xPathForProductTypeFilterButton)));
                    Console.WriteLine($"'Product Type' Filter Button Found: {productTypeButtonFound}");

                    if (productTypeButtonFound)
                    {
                        Console.WriteLine("Clicking Product Type Filter Button.");
                        new Actions(_driver).Click(productTypeButtonElement).Perform();

                        var xPathForProductType = $"//div[@id='refinement--producttype']/ul/li[@data-filter-name='{productType}']/button";

                        Console.WriteLine($"Searching for Product Type Filter For '{productType}'");
                        var (productTypeFound, productTypeElement) = _driver.WaitToGetWebElement(TimeSpan.FromSeconds(2), _ => _.FindElement(By.XPath(xPathForProductType)));
                        Console.WriteLine($"'{productType}' Filter Found: {productTypeFound}");

                        if (productTypeFound)
                        {
                            Console.WriteLine($"Clicking Product Type Filter For Product Type: {productType}");
                            new Actions(_driver ).Click(productTypeElement).Perform();

                            //TODO: Close Filters
                            return true;
                        }
                        else
                        {
                            Console.WriteLine($"Not able to find button for Product Type: {productType}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not able to find product type filter button.");
                    }
                }
                else
                {
                    Console.WriteLine("Not able to find filter button.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception Thrown When Attempting to Filter By Product Type: {productType}");
                Console.WriteLine($"Exception Type: {e.GetType()}");
                Console.WriteLine($"Exception Message: {e.Message}");
            }

            return false;
        }
    }
}
