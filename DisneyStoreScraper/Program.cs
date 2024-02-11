using DisneyStoreScraper;
using OpenQA.Selenium;

var driver = WebDriverFactory.NewWebDriver();

var disneyScraper = new DisneyScraper(driver);

try
{
    Console.WriteLine("Navigating to Disney Store.");

    driver.Navigate().GoToUrl("https://www.shopdisney.com/");

    var xPathForPopupIFrame = "//iframe[@id='attentive_creative']";

    disneyScraper.ClosePopupIfDisplayed(xPathForPopupIFrame);

    var wasAbleToSearch = disneyScraper.SearchStoreForText("Star Wars");

    if (wasAbleToSearch)
    {
        var wasAbleToFilter = disneyScraper.FilterSearchResultsByProductType("action figures");

        if(wasAbleToFilter)
        {
            var (isProductsFound, products) = driver.WaitToGetWebElement(TimeSpan.FromSeconds(2), _ => _.FindElement(By.ClassName("search__items")));

            Console.WriteLine($"\nProducts Found: {isProductsFound}");
            if (isProductsFound)
            {
                var productLinks = products!.FindElements(By.ClassName("product__tile_full_link"));                

                if (productLinks.Any())
                {
                    var productUrls = productLinks.Take(3).Select(_ => _.GetAttribute("href")).ToArray();

                    foreach (var url in productUrls)
                    {
                        var product = disneyScraper.ScrapeProductFromPage(url);

                        Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine($"\nProduct Page: {product.Url}");
                        Console.WriteLine($"\nProduct Name: {product.Name}");
                        Console.WriteLine($"\nProduct Price: {product.Price}");
                        Console.WriteLine($"\nProduct Rating: {product.Rating}");
                        Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    }
                }
            }
        }
    }
 }
catch (Exception e)
{
    Console.WriteLine($"Exception {e.GetType()} thrown");
    Console.WriteLine(e.Message);
    driver.Quit();
}
finally
{
    driver.Quit();
}

Console.WriteLine("\nPress any key to close the application");
Console.ReadKey();