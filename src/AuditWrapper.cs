using EAScraperJob.Data;
using EAScraperJob.Dtos;
using EAScraperJob.Interfaces;

namespace EAScraperJob
{
    public class AuditWrapper : IAuditWrapper
    {
        private DataContext _context;
        public AuditWrapper(DataContext context)
        {
            _context = context;
        }

        public async Task SaveToDB(Audit audit)
        {
            try
            {
                await _context.AddAsync(audit);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
