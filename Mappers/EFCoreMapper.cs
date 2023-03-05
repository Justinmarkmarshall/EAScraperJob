using EAScraperJob.Dtos;
using EAScraperJob.Models;

namespace EAScraperJob.Mappers
{
    public static class EFCoreMapper
    {
        public static List<Property> Map(this IList<House> houses)
        {
            var properties = new List<Property>();

            foreach (var house in houses)
            {
                try
                { 
                    var prx = Double.TryParse(house.Price.Replace("£", "").Replace(",", ""), out var price) ? price : 0;

                    //price POA causes the price to break
                    properties.Add(new Property()
                    {
                        Description = house.Description,
                        Price = prx,
                        Area = string.IsNullOrWhiteSpace(house.Address.Area) ? "" : house.Address.Area,
                        Link = house.Link,
                        Deposit = house.Deposit, 
                        Date = DateTime.Now, 
                        Postcode = house.Address.PostCode,
                        DateListed = house.DateListed,
                        DateReduced = house.DateReduced,
                        Reduced = house.Reduced
                    });
                }
                catch (Exception ex)
                {

                }
            }

            return properties;
        }

        public static Audit Map(this int postCodeCounter, string postcode, 
            string price, Enums.EstateAgent site)
        {
            return new Audit()
            {
                Site = (int)site,
                Postcode = postcode,
                UniqueResultsCount = postCodeCounter,
                Price = Convert.ToInt32(price),
                Date = DateTime.Now
            };
        }

        public static Audit Map(this string postcode, string zooplaCode, string title, Enums.EstateAgent site = Enums.EstateAgent.RightMove)
        => new()
        {
            Date = DateTime.Now,
            Site = (int)site,
            Postcode = $"postcode {postcode} no longer maps to zooplaCode {zooplaCode}, it now maps to {title}",
            Price = 0,
            UniqueResultsCount = 0
        };

        public static Log Map(this Exception ex)
        {
            return new Log()
            {
                Message = ex.Message,
                Source = ex.Source,
                StackTrace = ex.StackTrace.ToCharArray().Take<char>(100).ToString(),
                HelpLink = ex.HelpLink,
                TargetSite = ex.TargetSite?.ToString(),
                Date = DateTime.Now
            };
        }

        public static Log Map(this string message)
        {
            return new Log()
            {
                Message = message,
                Date = DateTime.Now
            };
        }
    }
}
