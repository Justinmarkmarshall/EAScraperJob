using EAScraperJob.Models;

namespace EAScraperJob.Interfaces
{
    public interface IRightMoveScraper
    {
        public Task<IList<House>> GetProperties(string price);
    }
}
