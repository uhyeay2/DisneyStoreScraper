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
                var (searchBoxClicked, _) = ClickElementHelper.TryToClickWithConsoleLogs(_driver, "Search Box", TimeSpan.FromSeconds(2), 
                    _ => _.FindElement(By.XPath("//section[@class='siteSearch']/button")));

                if (!searchBoxClicked)
                {
                    return false;
                }

                var (searchFormClicked, searchFormElement) = ClickElementHelper.TryToClickWithConsoleLogs(_driver, $"Form To Input Text: {textToSearchFor}", TimeSpan.FromSeconds(2), 
                    _ => _.FindElement(By.XPath("//form[@role='search']/div/input[@type='search']")));
                
                if(!searchFormClicked)
                {
                    return false;
                }

                Console.WriteLine($"\nSending Keys To Text Input: {textToSearchFor}");
                try
                {
                    searchFormElement!.SendKeys(textToSearchFor);
                    Console.WriteLine("Text Entered Into Form Input.");
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed To Send Keys To Text Input.");
                    return false;
                }

                var (submitButtonClicked, _) = ClickElementHelper.TryToClickWithConsoleLogs(_driver, "Submit Search Button", TimeSpan.FromSeconds(2), 
                    _ => _.FindElement(By.XPath("//form[@role='search']/div/button")));

                return submitButtonClicked;
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
                var (filterButtonFound, _) = ClickElementHelper.TryToClickWithConsoleLogs(_driver, "Filter Button", TimeSpan.FromSeconds(2), 
                    _ => _.FindElement(By.XPath("//div[@class='search__filter_sort']/button[@name='search__filter_btn']")));

                if (!filterButtonFound)
                {
                    return false;
                }

                var (productTypeFilterFound, _) = ClickElementHelper.TryToClickWithConsoleLogs(_driver, "Product Type Filter", TimeSpan.FromSeconds(2),
                    _ => _.FindElement(By.XPath("//div[@class='refinements']/div/header/button[@title='Product Type']")));

                if (!productTypeFilterFound)
                {
                    return false;
                }

                var (specificProductTypeFilterFound, _) = ClickElementHelper.TryToClickWithConsoleLogs(_driver, $"Specific Product Type Filter: {productType}", TimeSpan.FromSeconds(2),
                    _ => _.FindElement(By.XPath($"//div[@id='refinement--producttype']/ul/li[@data-filter-name='{productType}']/button")));

                if (!specificProductTypeFilterFound)
                {
                    return false;
                }

                // wait for clear filters button to be visible before trying to move forward
                _driver.WaitToGetWebElement(TimeSpan.FromSeconds(2), _ => _.FindElement(By.ClassName("clear-filters")));

                var (seeProductsButtonClicked, _) = ClickElementHelper.TryToClickWithConsoleLogs(_driver, "See Products Button", TimeSpan.FromSeconds(2),
                    _ => _.FindElement(By.ClassName("see-products")));

                return seeProductsButtonClicked;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception Thrown When Attempting to Filter By Product Type: {productType}");
                Console.WriteLine($"Exception Type: {e.GetType()}");
                Console.WriteLine($"Exception Message: {e.Message}");
            }

            return false;
        }

        public DisneyProduct ScrapeProductFromPage(string url)
        {
            _driver.Navigate().GoToUrl(url);
            Console.WriteLine($"\nMoving To Product Page: {url}");

            Console.WriteLine("Scrolling Down.");
            var amountToScroll = _driver.Manage().Window.Size.Height / 2;
            new Actions(_driver).ScrollByAmount(0, amountToScroll).Perform();

            ClickElementHelper.TryToClickWithConsoleLogs(_driver, "Popup", TimeSpan.FromSeconds(10), _ => _.FindElement(By.ClassName("dw-modal__close")));

            Console.WriteLine("Scraping Data");

            var productDetailContent = _driver.FindElement(By.XPath("//div[@class='product-detail-content-section']"));

            var name = productDetailContent.FindElement(By.ClassName("product-name")).Text;

            var price = productDetailContent.FindElement(By.ClassName("price")).Text;

            string rating;
            try
            {
                rating = productDetailContent.FindElement(By.XPath("//div[@class='ratings']/div/div/div/div[2]")).GetAttribute("textContent");
            }
            catch (NoSuchElementException)
            {
                rating = "No Ratings Found";
            }

            var product = new DisneyProduct(url, name, price, rating);

            return product;
        }
    }
}

/*
 <div class="bv_avgRating_component_container notranslate">4.6</div>
 */
