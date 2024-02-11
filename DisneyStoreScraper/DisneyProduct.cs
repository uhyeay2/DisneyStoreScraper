namespace DisneyStoreScraper
{
    public class DisneyProduct
    {
        public DisneyProduct(string url, string name, string price, string rating)
        {
            Url = url;
            Name = name;
            Price = price;
            Rating = rating;
        }

        public string Url { get; set; }

        public string Name { get; set; }

        public string Price { get; set; }

        public string Rating { get; set; }
    }
}
