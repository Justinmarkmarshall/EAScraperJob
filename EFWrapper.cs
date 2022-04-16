using EAScraperJob.Data;
using EAScraperJob.Dtos;
using EAScraperJob.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EAScraperJob
{
    public class EFWrapper : IEFWrapper
    {
        private readonly DataContext _context;

        public EFWrapper(DataContext context)
        {
            _context = context;
        }

        public async Task SaveToDB(List<Property> properties)
        {    
            try
            {
                await _context.AddRangeAsync(properties);
                await _context.SaveChangesAsync();
            }
           catch (Exception ex)
            {

            }
        }

        public async Task<List<Property>> GetFromDB()
        {
            return await _context.Properties.ToListAsync();
        }

        public async Task SaveToDB(Audit audit)
        {
            await _context.AddAsync(audit);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Audit>> GetAuditFromDB()
        {
            return await _context.Audit.ToListAsync();
        }

        public async Task<List<Property>> GetByPrice(double price)
        {
            return await _context.Properties.Where(r => r.Price < price && r.Price > price - 12000).ToListAsync();
        }

        public async Task SaveToDB(Log log)
        {
            await _context.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
