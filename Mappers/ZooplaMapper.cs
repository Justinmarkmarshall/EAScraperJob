using AngleSharp.Dom;
using EAScraperJob.Models;

namespace EAScraperJob.Mappers
{
    public static class ZooplaMapper
    {
        public static List<House> MapZ(this IHtmlCollection<IElement> searchResult)
        {
            var lstReturn = new List<House>();

            foreach (var child in searchResult [0].Children)
            {
                var pid = "0";
                var allINeed = child.InnerHtml;
                var prce = child.GetElementsByClassName("css-1o565rw-Text eczcs4p0");
                var address = child.GetElementsByClassName("css-nwapgq-Text eczcs4p0");
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
                        //Deposit = Calculate10PcOfPrice(prce[0].InnerHtml.Remove(0, 1).Replace("£", "").Replace(",", ""))
                        Deposit = 0
                    });
            }
            return lstReturn;
        }

        //was blowing up with 179999
        private static int Calculate10PcOfPrice(string innerHtml) => Convert.ToInt32(innerHtml) / 10;

        private static int CalculateMonthlyRepayment(string price)
        {
            var prx = Convert.ToInt32(price);
            return Convert.ToInt32(Math.Floor(prx / 236M));
        }
        
        private static int Calculate10PcOffPrice(int price) => price - (price / 100 * 10);
    }
}
