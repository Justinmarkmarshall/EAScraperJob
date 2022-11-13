namespace EAScraperJob.Models
{
    public class House
    {
        public string Description { get; set; }
        public string Price { get; set; }
        public string? Area { get; set; }
        public List<string>? Images { get; set; }
        public string Link { get; set; }
        public int Deposit { get; set; }
        public Address Address { get; set; }
        public string DateScraped { get; set; }
    }
}
