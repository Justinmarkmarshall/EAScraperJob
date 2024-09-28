using EAScraperJob.Models;

namespace EAScraperJob.Interfaces
{
    public interface IZooplaScraper
    {
        public Task<IList<House>> GetProperties(string price);
    }
}
