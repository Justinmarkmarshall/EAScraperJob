using System.ComponentModel.DataAnnotations;

namespace EAScraperJob.Dtos
{
    public class Audit
    {
        [Key]
        public int Id { get; set; }

        public string Postcode { get; set; } = String.Empty;

        public int Price { get; set; }

        public int Site { get; set; } = 0;

        public int UniqueResultsCount { get; set; }

        public DateTime Date { get; set; }
    }
}
