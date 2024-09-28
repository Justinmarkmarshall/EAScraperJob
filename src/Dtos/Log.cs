using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAScraperJob.Dtos
{
    public class Log
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; } = "";
        public string? Source { get; set; }
        public string? StackTrace { get; set; }
        public string? HelpLink { get; set; }
        public string? TargetSite { get; set; }
        public DateTime Date { get; set; }
    }
}
