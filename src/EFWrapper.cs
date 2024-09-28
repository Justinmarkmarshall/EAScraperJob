using EAScraperJob.Data;
using EAScraperJob.Dtos;
using EAScraperJob.Interfaces;
using Microsoft.EntityFrameworkCore;
using dto = EAScraperJob.Dtos;
using db = EAScraperJob.Data.DataContext;

namespace EAScraperJob
{
    public class EFWrapper : IEFWrapper
    {
        private readonly DataContext _context;

        public EFWrapper(DataContext context)
        {
            _context = context;
        }

        public async Task UpsertProperties(List<dto.Property> properties)
        {
            Property foobarProperty = null;
            try
            {
                var unique = new List<Property>();
                foreach (dto.Property property in properties)
                {

                    var existingProperty = _context.Properties.Where(r => r.Link == property.Link);

                    if (existingProperty.Any())
                    {
                        var test = existingProperty.FirstOrDefault();

                        if (test.Deleted.HasValue)
                        {
                            if (test.Deleted.Value)
                            {
                                continue;
                            }
                            
                        }

                        if (existingProperty!.FirstOrDefault()!.Price > property.Price)
                        {
                            existingProperty.FirstOrDefault()!.Price = property.Price;
                            existingProperty.FirstOrDefault()!.DateListed = existingProperty.FirstOrDefault()!.Date;
                            existingProperty.FirstOrDefault()!.Date = DateTime.Now;
                            existingProperty.FirstOrDefault()!.Reduced = true;
                            _context.Update(existingProperty);
                        }
                    }
                    else
                    {                        
                        unique.Add(property);
                        continue;
                    }                    
                }

                await _context.AddRangeAsync(unique);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Succesfully wrote {unique.Count()} to the DB");
            }
             catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}, {ex.InnerException}, property: {foobarProperty!.Link}");
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
