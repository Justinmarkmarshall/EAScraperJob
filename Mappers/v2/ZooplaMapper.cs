using AngleSharp.Dom;
using EAScraperJob.Models;

namespace EAScraperJob.Mappers.v2
{
    public static class ZooplaMapper
    {
        public static List<House> v2MapZ(this IHtmlCollection<IElement> searchResult)
        {
            var lstReturn = new List<House>();

            foreach (var child in searchResult[0].Children)
            {
                var pid = "0";
                var allINeed = child.InnerHtml;
                var prce = child.GetElementsByClassName("css-xz7r6w-Price e9kuaf13");
                var address = child.GetElementsByClassName("css-1uvt63a-Address e9kuaf11");
                var link = child.GetElementsByTagName("e2uk8e5 css-15xcaqt-StyledLink-Link-ImageLink");
                //var description = child.GetElementsByTagName("img")[0].OuterHtml.Split("=")[3];
                var aTags = child.QuerySelector("a")?.OuterHtml.Split("=");
                if (aTags != null)
                {
                    pid = aTags[2]?.Split("/")[3];
                }

                if (!allINeed.ToLower().Contains("hotel")
                    && !allINeed.ToLower().Contains("retirement")
                    && !allINeed.ToLower().Contains("investment only")
                    && !allINeed.ToLower().Contains("cash buyers only")
                    && !allINeed.ToLower().Contains("shared ownership")
                    && !allINeed.ToLower().Contains("share")) lstReturn.Add(new House()
                    {
                        Price = prce[0].InnerHtml,
                        Area = address[0].InnerHtml,
                        Link = $"https://www.zoopla.co.uk/for-sale/details/{pid}",
                        //MonthlyRepayments = CalculateMonthlyRepayment(prce[0].InnerHtml.Replace("£", "").Replace(",", "")),
                        MonthlyRepayments = 0,
                        //Deposit = Calculate10PcOfPrice(prce[0].InnerHtml.Remove(0, 1).Replace("£", "").Replace(",", ""))
                        Deposit = 0
                    });
            }
            return lstReturn;
        }
    }
}
