using EAScraperJob.Dtos;

namespace EAScraperJob.Interfaces
{
    public interface IAuditWrapper
    {
        public Task SaveToDB(Audit audit);
    }
}
