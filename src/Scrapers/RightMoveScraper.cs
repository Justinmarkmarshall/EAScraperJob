using EAScraperJob.Interfaces;
using EAScraperJob.Mappers;
using EAScraperJob.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;
using EAScraperJob.Config;

namespace EAScraperJob.Scrapers
{
    public class RightMoveScraper : IRightMoveScraper
    {
        private readonly IAngleSharpWrapper _angleSharpWrapper;
        private readonly IAuditWrapper _auditWrapper;        
        private readonly PropertyPreferences _propertyPreferences;
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
            {"NP20", "5E991" },
        };      
        private Dictionary<string, string> _affordablePostcodesCloseToNewportOrLondon = new Dictionary<string, string>()
        {
            { "Newport", "5E991" },
            { "Farnham", "5E506" },
            { "Aldershot", "5E22" }, 
            { "Camberley", "5E272" }, 
            { "Cardiff", "5E281" }, 
            { "Grays", "5E571" },
            { "Basildon", "5E114" }
            
        };        
        private Dictionary<string, string> _affordablePostcodesCloseToNewport = new Dictionary<string, string>()
        {
            { "Newport", "5E991" },
            { "Cardiff", "5E281" }, 
            { "Swansea", "5E281" }
        };
        //private Dictionary<string, Postcode> _commuterBelt = new Dictionary<string, Postcode>()
        //{
        //    { "Luton", new Postcode{ RightMoveRef = "5E876", PriceBand = 140000 }},
        //    { "Grays", new Postcode{ RightMoveRef = "5E571", PriceBand = 140000 }},
        //    { "Basildon", new Postcode{ RightMoveRef = "5E114", PriceBand = 140000 }},
        //    { "Dartford", new Postcode{ RightMoveRef = "5E407", PriceBand = 140000 }},
        //    { "Purfleet", new Postcode{ RightMoveRef = "5E7469", PriceBand = 140000 }},
        //};
        private Dictionary<string, Postcode> _hereToHeathrow = new Dictionary<string, Postcode>()
        {
            { "Acton",  new Postcode{ RightMoveRef = "5E85314", PriceBand = 250000 } },
            { "Hounslow", new Postcode{ RightMoveRef = "5E66885", PriceBand = 250000 }},
            { "Putney", new Postcode{ RightMoveRef = "5E85244", PriceBand = 250000 }},
            { "Ealing", new Postcode{ RightMoveRef = "5E87504", PriceBand = 250000 }},
            { "Hanwell", new Postcode{ RightMoveRef = "5E70328", PriceBand = 250000 }},
            { "Isleworth", new Postcode{ RightMoveRef = "5E715", PriceBand = 250000 }},
            { "Richmond", new Postcode{ RightMoveRef = "5E61415", PriceBand = 250000 }},
            { "Limehouse", new Postcode{ RightMoveRef = "5E85365", PriceBand = 250000 }},
        };
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
            { "W3", "5E87504" },
            { "HA9", "5E79781" },

        };

        private Dictionary<string, Postcode> _niceAroundMe = new Dictionary<string, Postcode>()
        {
            { "Acton",  new Postcode{ RightMoveRef = "5E85314", PriceBand = 250000 } },
            { "Putney", new Postcode{ RightMoveRef = "5E85244", PriceBand = 250000 }},
            { "Ealing", new Postcode{ RightMoveRef = "5E87504", PriceBand = 250000 }},
            { "Hanwell", new Postcode{ RightMoveRef = "5E70328", PriceBand = 250000 }},
            { "Isleworth", new Postcode{ RightMoveRef = "5E715", PriceBand = 250000 }},
            { "Richmond", new Postcode{ RightMoveRef = "5E61415", PriceBand = 250000 }},
        };

        private Dictionary<string, Postcode> _twoBedsInGreater = new Dictionary<string, Postcode>()
        {
            { "Belvedere",  new Postcode{ RightMoveRef = "5E137", PriceBand = 250000 } },
            //{ "Erith", new Postcode{ RightMoveRef = "5E489", PriceBand = 250000 }},
            //{ "Dartford", new Postcode{ RightMoveRef = "5E407", PriceBand = 250000 }},
            //{ "Morden", new Postcode{ RightMoveRef = "5E954", PriceBand = 250000 }},
            //{ "Basildon", new Postcode{ RightMoveRef = "5E114", PriceBand = 250000 }},
            //{ "Romford", new Postcode{ RightMoveRef = "5E1138", PriceBand = 250000 }},
            //{ "Shenfield", new Postcode{ RightMoveRef = "5E21938", PriceBand = 250000 }},
            { "Crystal Palace", new Postcode{ RightMoveRef = "5E70306", PriceBand = 250000 }},
            { "Brick Lane",  new Postcode{ RightMoveRef = "5E96864", PriceBand = 250000 } },
            { "Abbey Wood",  new Postcode{ RightMoveRef = "5E85240", PriceBand = 250000 } },
            { "Bethnal Green",  new Postcode{ RightMoveRef = "5E85224", PriceBand = 250000 } }, 
            { "Bristol",  new Postcode{ RightMoveRef = "5E219", PriceBand = 250000 } }, 
            { "Ilford",  new Postcode{ RightMoveRef = "5E674", PriceBand = 250000 } }
        };

        
        private Dictionary<string, Postcode> _brickLane = new Dictionary<string, Postcode>()
        {
            { "Brick Lane",  new Postcode{ RightMoveRef = "5E96864", PriceBand = 250000 } }
        };

        private Dictionary<string, Postcode> _leeds = new Dictionary<string, Postcode>()
        {
            {
                "Leeds", new Postcode{ RightMoveRef = "5E787", PriceBand = 100000 }
            }
        };

        public RightMoveScraper(IOptions<PropertyPreferences> propertyPreferences,
            IAngleSharpWrapper angleSharpWrapper, IAuditWrapper auditWrapper)
        {
            _angleSharpWrapper = angleSharpWrapper;
            _auditWrapper = auditWrapper;
            _propertyPreferences = propertyPreferences.Value;
        }
        //RIGHTMOVE HAVE A DIFFERENT LOCATION IDENTIFIER SYSTEM
        public async Task<IList<House>> GetProperties(int price)
        {
            var uniqueHouses = new List<House>();
            var radius = 5;
            //because when the radius is higher, you get more results and so only the highest ranking ones get collected, changing to 1 mile radius
            //if this is not getting enough, then could do a few queries with different radius'
            //got alot more unique results like this, may need to think about chopping off some postcodes
            //changing back to 2 miles to avoid clogging
            //need to fix the checker for 55s and retirements because getting flooded
            // 2909 changing again to look around leeds so setting radius to 5
            foreach (var location in _leeds)
            {
                var postCodeCounter = 0;
                for (int i = 1; i < radius; i++)
                { 
                    //region is inside london
                    string url = $"https://www.rightmove.co.uk/property-for-sale/find.html?searchType=SALE&locationIdentifier=REGION%{location.Value.RightMoveRef}&insId=1&radius={i}.0&minPrice={Calculate25PcOffPrice(price)}&maxPrice={price}&minBedrooms=2&maxBedrooms=5&displayPropertyType=&maxDaysSinceAdded=&_includeSSTC=on&sortByPriceDescending=&primaryDisplayPropertyType=&secondaryDisplayPropertyType=&oldDisplayPropertyType=&oldPrimaryDisplayPropertyType=&newHome=&auction=false";
                    var document = await _angleSharpWrapper.GetSearchResults(url);
                    if (!document.Title.ToLower().Contains(location.Key.ToLower()))
                    {
                        await _auditWrapper.SaveToDB(location.Key.Map(location.Value.RightMoveRef, document.Title));
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

                                var listingInfo = page.GetElementsByClassName("_2nk2x6QhNB1UrxdI5KpvaF");
                                if (listingInfo.Any())
                                {
                                    ProcessDates(home, listingInfo[0].TextContent.ToLower());
                                }
                                uniqueHouses.Add(home);
                                postCodeCounter++;
                            };
                        }
                    }
                    await _auditWrapper.SaveToDB(postCodeCounter.Map(location.Key, price.ToString(), Enums.EstateAgent.RightMove));
                }
            }

            //commenting out 25/05 because it's gotta be Acton baby
            //foreach (var location in _commuterBelt)
            //{
            //    var postCodeCounter = 0;
            //    for (int i = 1; i < 2; i++)
            //    {
            //        //instead of outcode in london, these places use regions
            //        string url = $"https://www.rightmove.co.uk/property-for-sale/find.html?searchType=SALE&locationIdentifier=REGION%{location.Value.RightMoveRef}&insId=1&radius={i}.0&minPrice={Calculate25PcOffPrice(price)}&maxPrice={price}&minBedrooms=0&maxBedrooms=2&displayPropertyType=flats&maxDaysSinceAdded=&_includeSSTC=on&sortByPriceDescending=&primaryDisplayPropertyType=&secondaryDisplayPropertyType=&oldDisplayPropertyType=&oldPrimaryDisplayPropertyType=&newHome=&auction=false";
            //        var document = await _angleSharpWrapper.GetSearchResults(url);
            //        //Doc title says Newport
            //        if (!document.Title.ToLower().Contains(location.Key.ToLower()))
            //        {
            //            await _auditWrapper.SaveToDB(location.Key.Map(location.Value.RightMoveRef, document.Title));
            //            continue;
            //        }
            //        var searchResults = document.GetElementsByClassName("l-searchResults");
            //        var properties = searchResults[0].Children;
            //        if (properties.Any())
            //        {
            //            var newHomes = properties.MapRM();
            //            foreach (var home in newHomes)
            //            {
            //                if (!uniqueHouses.Any(r => r.Link == home.Link))
            //                {
            //                    //go off and find out what type it is
            //                    //addition will add alot of runtime, but also improve data returned, can also expand upon this data
            //                    var page = await _angleSharpWrapper.GetSearchResults(home.Link);
            //                    home.Description = page.Title;

            //                    var listingInfo = page.GetElementsByClassName("_2nk2x6QhNB1UrxdI5KpvaF");
            //                    if (listingInfo.Any())
            //                    {
            //                        ProcessDates(home, listingInfo[0].TextContent.ToLower());
            //                    }
            //                    uniqueHouses.Add(home);
            //                    postCodeCounter++;
            //                };
            //            }
            //        }
            //        await _auditWrapper.SaveToDB(postCodeCounter.Map(location.Key, price.ToString(), Enums.EstateAgent.RightMove));
            //    }               
            //}
            return uniqueHouses;
        }



        private int CalculateMaxPrice(int price, string postcode)
        {
            if (postcode == "5E991")
            {
                return 80000;
            }
            return price;
        }
        private int Calculate25PcOffPrice(int price)
        {          
            return price - (price / 100 * 25);
        }

        private House ProcessDates(House house, string text)
        {
            if (text.Contains("added") || text.Contains("listed"))
            {
                // the date is the listed date
                house.DateListed = GetDate(text);
            }
            if (text.Contains("reduced"))
            {
                house.DateReduced = GetDate(text);
                house.Reduced = true;
            }
            return house;
        }

        private DateTime? GetDate(string text)
        {
            foreach (var word in text.Split(" "))
            {
                if (DateTime.TryParse(word.ToCharArray(), out DateTime result))
                {
                    return result;
                }
            }

            return null;
        }
    }
}
