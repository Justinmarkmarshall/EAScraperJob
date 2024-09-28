using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAScraperJob.Dtos
{
    [Table("Properties")]
    public class Property
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; } = String.Empty;
        public double Price { get; set; } = 0F;
        public string Area { get; set; } = String.Empty;
        public string Link { get; set; } = String.Empty;
        public int Deposit { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public bool Reduced { get; set; } = false;

        public DateTime? DateListed { get; set; } = default!;

        public DateTime? DateReduced { get; set; } = default!;

        public bool? Deleted { get; set; } = false;
    }
}
