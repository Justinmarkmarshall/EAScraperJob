using EAScraperJob.Interfaces;
using EAScraperJob.Mappers;
using EAScraperJob.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAScraperJob.Scrapers
{
    public class RightMoveScraper : IRightMoveScraper
    {
        IAngleSharpWrapper _angleSharpWrapper;
        private IEFWrapper _efWrapper;
        IAuditWrapper _auditWrapper;
        private Dictionary<string, string> _affordablePostCodes = new Dictionary<string, string>()
        {
            { "HP1", "5E1077" },
            { "E6", "5E759" },
            { "SE28", "5E2329" },
            { "N9", "5E1687" },
            { "SE2", "5E2320" },
            { "SE25", "5E2326" },
            { "E12", "5E747" },
            { "SE18", "5E2318" },
            { "SE20", "5E2321" },
            { "BR3", "5E253" },
            { "E13", "5E748" },
            { "PE1", "5E1975" },
            { "RM19", "5E2197" },
            { "NN29", "5E1794" },
            { "CV21", "5E559" },
            { "ME1", "5E1608" },
            { "AL10", "5E35" },
            { "RG1", "5E2137" },
            { "WD6", "5E2812" },
            { "SL0", "5E2375" },
            { "W3", "5E87504" }
        };

        //OUTCODE is the prefix used in the url when search with postcodes
        //REGION is the prefix used in the url when search with location
        //will go with postcodes because it matches more accurately what I am doing with Zoopla
        //it looks like they also respond to HEX
        //key only used in auditing
        //need more West and North and SouthWestpostcodes in here
        //codes to change so need a away of checking them
        private Dictionary<string, string> _londonPostCodes = new Dictionary<string, string>()
        {
            {"SW15", "5E2501" },
            {"SW14", "5E2500" },
            {"SW19", "5E2505" },
            { "NW9", "5E1865" },
            //{ "E6", "5E759" },
            { "SE28", "5E2329" },
            //{ "N9", "5E1687" },
            { "SE2", "5E2320" },
            { "SE25", "5E2326" },
            //{ "E12", "5E747" },
            { "SE18", "5E2318" },
            { "SE20", "5E2321" },
            //{ "E13", "5E748" },
            { "W3", "5E2763" },
            {"W32", "5E87504" }, 
            //{"HA8", "5E1061" },
            {"WD23", "5E2806" },
            {"W5", "5E2765" },
        };

        public RightMoveScraper(IAngleSharpWrapper angleSharpWrapper, IAuditWrapper auditWrapper)
        {
            _angleSharpWrapper = angleSharpWrapper;
            _auditWrapper = auditWrapper;
        }
        //RIGHTMOVE HAVE A DIFFERENT LOCATION IDENTIFIER SYSTEM


        public async Task<IList<House>> GetProperties(string price)
        {
            var uniqueHouses = new List<House>();

            //because when the radius is higher, you get more results and so only the highest ranking ones get collected, changing to 1 mile radius
            //if this is not getting enough, then could do a few queries with different radius'
            //got alot more unique results like this, may need to think about chopping off some postcodes
            foreach (var location in _londonPostCodes)
            {
                var postCodeCounter = 0;
                string url = $"https://www.rightmove.co.uk/property-for-sale/find.html?searchType=SALE&locationIdentifier=OUTCODE%{location.Value}&insId=1&radius=1.0&minPrice={Calculate25PcOffPrice(Convert.ToInt32(price))}&maxPrice={price}&minBedrooms=0&maxBedrooms=1&displayPropertyType=flats&maxDaysSinceAdded=&_includeSSTC=on&sortByPriceDescending=&primaryDisplayPropertyType=&secondaryDisplayPropertyType=&oldDisplayPropertyType=&oldPrimaryDisplayPropertyType=&newHome=&auction=false";
                var document = await _angleSharpWrapper.GetSearchResults(url);
                if (!document.Title.ToLower().Contains(location.Key.ToLower()))
                {
                    await _auditWrapper.SaveToDB(location.Key.Map(location.Value, document.Title));
                    continue;
                }
                var searchResults = document.GetElementsByClassName("l-searchResults");
                var properties = searchResults[0].Children;
                if (properties.Any())
                {
                    var newHomes = properties.MapRM();
                    foreach (var home in newHomes)
                    {
                        if (!uniqueHouses.Any(r => r.Link == home.Link))
                        {
                            //go off and find out what type it is
                            //addition will add alot of runtime, but also improve data returned, can also expand upon this data
                            var page = await _angleSharpWrapper.GetSearchResults(home.Link);
                            home.Description = page.Title;

                            uniqueHouses.Add(home);
                            postCodeCounter++;
                        };
                    }
                }
                await _auditWrapper.SaveToDB(postCodeCounter.Map(location.Key, price, Enums.EstateAgent.RightMove));
            }
            return uniqueHouses;
        }

        private int Calculate10PcOffPrice(int price) => price - (price / 100 * 10);
        private int Calculate25PcOffPrice(int price) => price - (price / 100 * 25);       
    }
}
