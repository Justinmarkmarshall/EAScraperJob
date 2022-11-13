using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EAScraperJob.Models;
using EAScraperJob.Models;

namespace EAScraperJob.Mappers.v2
{
    public static class ZooplaMapper
    {
        private const string PriceClass = "css-xz7r6w-Price";
        private const string AddressClass = "css-1uvt63a-Address";
        public static List<House> v2MapZ(this IHtmlCollection<IElement> propertiesDiv)
        {
            var lstReturn = new List<House>();

            foreach (var propertyCard in propertiesDiv[0].Children)
            {
                var pid = "0";
                var aTags = propertyCard.QuerySelector("a")?.OuterHtml.Split("=");
                if (aTags != null)
                {
                    pid = aTags[2]?.Split("/")[3];
                }

                var imgs = propertyCard.GetElementsByTagName("picture");

                var description = propertyCard.GetElementsByTagName("h2")[0].InnerHtml;
                var price = "";
                if (propertyCard.GetElementsByClassName(PriceClass).Any())
                {
                    price = propertyCard.GetElementsByClassName(PriceClass)[0].InnerHtml;
                }
                var area = "";
                if (propertyCard.GetElementsByClassName(AddressClass).Any())
                {
                    area = propertyCard.GetElementsByClassName(AddressClass)[0].InnerHtml;
                }

                var images = propertyCard.GetElementsByTagName("picture").Map();

                if (!propertyCard.InnerHtml.ToLower().Contains("hotel")
                    && !propertyCard.InnerHtml.ToLower().Contains("retirement")
                    && !propertyCard.InnerHtml.ToLower().Contains("investment only")
                    && !propertyCard.InnerHtml.ToLower().Contains("cash buyers only")
                    && !propertyCard.InnerHtml.ToLower().Contains("shared ownership")
                    && !propertyCard.InnerHtml.ToLower().Contains("share")) lstReturn.Add(new House()
                    {
                        Description = description,
                        Price = price,
                        Area = area,
                        Images = images,
                        Link = $"https://www.zoopla.co.uk/for-sale/details/{pid}",
                        Deposit = 0
                    });
            }
            return lstReturn;
        }

        private static List<string> Map(this IHtmlCollection<IElement> pics)
        {
            var images = new List<string>();

            foreach (var pic in pics)
            {
                foreach (IHtmlSourceElement img in pic.GetElementsByTagName("source"))
                {
                    string sourceSet = img.SourceSet;
                    foreach (string url in sourceSet.Split(" "))
                    {
                        if (url.Contains("jpg"))
                        {
                            images.Add(url);
                        }
                    }
                }
            }
            return images;
        }
    }
}
