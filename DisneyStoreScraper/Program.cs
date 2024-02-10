using DisneyStoreScraper;

var driver = WebDriverFactory.NewWebDriver();

var disneyScraper = new DisneyScraper(driver);

try
{
    Console.WriteLine("Navigating to Disney Store.");

    driver.Navigate().GoToUrl("https://www.shopdisney.com/");

    var xPathForPopupIFrame = "//iframe[@id='attentive_creative']";

    disneyScraper.ClosePopupIfDisplayed(xPathForPopupIFrame);

    var textToSearchFor = "Star Wars";

    disneyScraper.SearchStoreForText(textToSearchFor);

    var filterToApply = "action figures";

    disneyScraper.FilterSearchResultsByProductType(filterToApply);
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

Console.WriteLine("Press any key to close the application");
Console.ReadKey();