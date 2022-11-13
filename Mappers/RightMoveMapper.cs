using AngleSharp.Dom;
using EAScraperJob.Models;

namespace EAScraperJob.Mappers
{
    public static class RightMoveMapper
    {
        public static List<House> MapRM(this IHtmlCollection<IElement> searchResult)
        {
            var houses = new List<House>();

            searchResult.Count();

            for (int i = 0; i < searchResult.Count(); i++)
            {
                IElement property = searchResult[i].Children[0];
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
                        Address = address.FormatAddress(),
                        Link = $"https://www.rightmove.co.uk/properties/{aTags.Replace("prop", "")}#/?channel=RES_BUY",
                        Price = pce
                    });
            }

            #region deprecatedCode
            //foreach (var property in searchResult[0].Children)
            //{
            //    var allINeed = property.InnerHtml;

            //    var address = property.GetElementsByClassName("propertyCard-details")[0].
            //        GetElementsByClassName("propertyCard-address")[0].GetElementsByTagName("span")[0].InnerHtml;
            //    var pce = property.GetElementsByClassName("propertyCard-priceValue")[0].InnerHtml;
            //    var aTags = property.QuerySelector("a").Id;
            //    if (!allINeed.ToLower().Contains("hotel")
            //        && !allINeed.ToLower().Contains("retirement")
            //        && !allINeed.ToLower().Contains("investment only")
            //        && !allINeed.ToLower().Contains("cash buyers only")
            //        && !allINeed.ToLower().Contains("shared ownership")
            //        && !allINeed.ToLower().Contains("share")) houses.Add(new House()
            //        {
            //            Area = address.ToString(),
            //            Link = $"https://www.rightmove.co.uk/properties/{aTags.Replace("prop", "")}#/?channel=RES_BUY",
            //            Price = pce
            //        });                
            //}
            #endregion DeprecatedCode
            return houses;
        }

        private static Address FormatAddress(this string address)
        {
            var addressComponents = address.Split(",");

            Address formattedAddress = new Address();

            for (int i = addressComponents.Count() - 1; i >= 0; i--)
            {
                if (addressComponents[i].Any(char.IsDigit) && string.IsNullOrWhiteSpace(formattedAddress.PostCode))
                {
                    formattedAddress.PostCode = String.Concat(addressComponents[i].Replace(",", "").Where(c => !Char.IsWhiteSpace(c)));
                }
                else if (addressComponents[i].Any(char.IsDigit))
                {
                    //first line of address
                    formattedAddress.FirstLine = addressComponents[i];
                }
                else if (string.IsNullOrWhiteSpace(formattedAddress.Area) && !addressComponents[i].Contains("london", StringComparison.OrdinalIgnoreCase))
                {
                    // if it's the first or second one of the array, without numbers then it should be the area
                    formattedAddress.Area = String.Concat(addressComponents[i].Replace(",", "").Where(c => !Char.IsWhiteSpace(c)));
                }
            }

            return formattedAddress;
        }
    }
}
