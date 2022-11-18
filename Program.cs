// See https://aka.ms/new-console-template for more information
using EAScraperJob.Data;
using EAScraperJob.Interfaces;
using EAScraperJob.Mappers;
using EAScraperJob.Models;
using EAScraperJob.Scrapers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EAScraperJob
{
    public class Program
    {
        private const string price = "250000";
        static async Task Main(string[] args)
        {


            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false)
               .Build();

            var serviceCollection = new ServiceCollection()
                
                .AddDbContext<DataContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                })
                .AddScoped<DbContext, DataContext>()
                .AddTransient<IZooplaScraper, ZooplaScraper>()
                .AddTransient<IAngleSharpWrapper, AngleSharpWrapper>()
                .AddTransient<IRightMoveScraper, RightMoveScraper>()
                .AddTransient<IEFWrapper, EFWrapper>()
                .AddTransient<IAuditWrapper, AuditWrapper>()
                .AddLogging()
                .BuildServiceProvider();
            var efWrapper = serviceCollection.GetService<IEFWrapper>();

            try
            {
                await efWrapper.SaveToDB("Starting the job".Map());

                //setup ZooplaScraper
                var rmScraper = serviceCollection.GetService<IRightMoveScraper>();
                var zoopScraper = serviceCollection.GetService<IZooplaScraper>();                
                var rmResults = await rmScraper.GetProperties(price);
              
                //var zoopResults = await zoopScraper.GetProperties(price);

                var results = new List<House>();
                results.AddRange(rmResults);
                //results.AddRange(zoopResults);

                if (results.Any())
                {
                    //remove unique because I think that the pids are reused
                    //don't need to do this, can put into the upsert function
                    //var uniqueResults = await RemoveDuplicates(results.ToList(), efWrapper);
                    
                    await efWrapper.UpsertProperties(results.Map());
                    
                    //await efWrapper.SaveToDB(results.Map());
                }
            }
            catch (Exception ex)
            {
                
                //mapper here is throwing an error
                await efWrapper.SaveToDB(ex.Map());
            }
            
        }

        private static async Task<List<House>> RemoveDuplicates(List<House> houses, IEFWrapper efWrapper)
        {
            var existingProperties = await efWrapper.GetFromDB();

            var uniqueHomes = new List<House>();

            foreach (var house in houses)
            {
                if (!existingProperties.Select(r => r.Link).Contains(house.Link))
                {
                    uniqueHomes.Add(house);
                }
            }
            return uniqueHomes;
        }
    }
}
