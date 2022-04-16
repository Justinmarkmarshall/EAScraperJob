using AngleSharp.Dom;
using EAScraperJob.Models;

namespace EAScraperJob.Mappers
{
    public static class RightMoveMapper
    {
        public static List<House> MapRM(this IHtmlCollection<IElement> searchResult)
        {
            var houses = new List<House>();

            foreach (var property in searchResult[0].Children)
            {
                var allINeed = property.InnerHtml;

                var address = property.GetElementsByClassName("propertyCard-details")[0].
                    GetElementsByClassName("propertyCard-address")[0].GetElementsByTagName("span")[0].InnerHtml;
                var pce = property.GetElementsByClassName("propertyCard-priceValue")[0].InnerHtml;
                var aTags = property.QuerySelector("a").Id;
                if (!allINeed.ToLower().Contains("hotel")
                    && !allINeed.ToLower().Contains("retirement")
                    && !allINeed.ToLower().Contains("investment only")
                    && !allINeed.ToLower().Contains("cash buyers only")
                    && !allINeed.ToLower().Contains("shared ownership")
                    && !allINeed.ToLower().Contains("share")) houses.Add(new House()
                    {
                        Area = address.ToString(),
                        Link = $"https://www.rightmove.co.uk/properties/{aTags.Replace("prop", "")}#/?channel=RES_BUY",
                        Price = pce
                    });                
            }
            return houses;
        }
    }
}
