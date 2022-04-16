using EAScraperJob.Models;

namespace EAScraperJob.Interfaces
{
    public interface IExcelSaver
    {
        public void SaveToExcel(List<House> properties);
    }
}
