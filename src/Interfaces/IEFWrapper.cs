using EAScraperJob.Dtos;

namespace EAScraperJob.Interfaces
{
    public interface IEFWrapper
    {
        public Task UpsertProperties(List<Property> properties);

        public Task<List<Property>> GetFromDB();

        public Task SaveToDB(Audit audit);

        public Task<List<Property>> GetByPrice(double price);

        public Task SaveToDB(Log log);
    }
}
