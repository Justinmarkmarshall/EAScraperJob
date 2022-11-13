using EAScraperJob.Interfaces;
using EAScraperJob.Mappers;
using EAScraperJob.Mappers.v2;
using EAScraperJob.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAScraperJob.Scrapers
{
    public class ZooplaScraper : IZooplaScraper
    {

        private Dictionary<string, string> _londonPostCodes = new Dictionary<string, string>()
        {
            { "Beckton", "E6" },
            { "Thamesmead", "SE28" },
            { "East Ham", "E12" },
            { "UpperEdmonton", "N9" },
            { "Plumstead", "SE18" },
            { "Acton", "W3" }
        };


        private Dictionary<string, string> _affordablePostCodes = new Dictionary<string, string>()
        {
            { "HemelHempstead", "HP1" },
            { "Beckton", "E6" },
            { "Thamesmead", "SE28" },
            { "UpperEdmonton", "N9" },
            { "AbbeyWood", "SE2" },
            { "South Norwood", "SE25" },
            { "East Ham", "E12" },
            { "Plumstead", "SE18" },
            { "Anerley", "SE20" },
            { "Penge", "BR3" },
            { "Plaistow", "E13" },
            { "Peterborough", "PE1" },
            { "Purfleet", "RM19" },
            { "Wellingborough", "NN29" },
            { "Rugby", "CV21" },
            { "Chatham", "ME1" },
            { "Hatfield", "AL10" },
            { "Reading", "RG1" },
            { "Borehamwood", "WD6" },
            { "Iver", "SL0" },
            { "Acton", "W3" }
        };

        private const string V3ClassName = "css-1itfubx e19tytbu0";
        private const string V2ClassName = "css-1anhqz4-ListingsContainer";

        IAngleSharpWrapper _angleSharpWrapper;
        ILogger<ZooplaScraper> _logger;
        IAuditWrapper _auditWrapper;

        public ZooplaScraper(IAngleSharpWrapper angleSharpWrapper, ILogger<ZooplaScraper> logger, IAuditWrapper auditWrapper)
        {
            _angleSharpWrapper = angleSharpWrapper;
            _logger = logger;
            _auditWrapper = auditWrapper;
        }

        public async Task<IList<House>> GetProperties(string price)
        {
            var uniqueHouses = new List<House>();

            foreach (var location in _affordablePostCodes)
            {
                var postCodeCounter = 0;
                string url = $"https://www.zoopla.co.uk/for-sale/flats/{location.Value.ToLower()}/?is_auction=false&is_shared_ownership=false&page_size=25&price_max={price}&price_min={Calculate25PcOffPrice(Convert.ToInt32(price))}&view_type=list&q={location.Value.Replace("-", "%")}&radius=15&results_sort=newest_listings&search_source=facets";
                var document = await _angleSharpWrapper.GetSearchResults(url);

                var searchResults = document.GetElementsByClassName(V3ClassName);
                if (searchResults.Any())
                {
                    //then also in the mapper the class names had changed, so basically this scraper breaks because htey are using randomised class names on their site
                    var newHomes = searchResults.v2MapZ();
                    foreach (var home in newHomes)
                    {
                        if (!uniqueHouses.Any(r => r.Link == home.Link))
                        {
                            uniqueHouses.Add(home);
                            postCodeCounter++;
                        };
                    }
                }
                await _auditWrapper.SaveToDB(postCodeCounter.Map(location.Value, price, Enums.EstateAgent.Zoopla));
            }

            return uniqueHouses;
        }

        private int Calculate10PcOffPrice(int price) => price - (price / 100 * 10);
        private int Calculate25PcOffPrice(int price) => price - (price / 100 * 25);
    }
}
