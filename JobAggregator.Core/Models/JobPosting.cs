
using System;
using System.ComponentModel.DataAnnotations;

namespace JobAggregator.Core.Models
{
    public class JobPosting
    {
        [Key]
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Link { get; set; }
        public DateTime PublishedDate { get; set; }
        public required string Summary { get; set; }
        public required string JobType { get; set; } // Contract, Permanent
        public required string JobMode { get; set; } // Remote, Hybrid, Onsite
        public decimal? Salary { get; set; }
        public string? SalaryUnit { get; set; } // Per Annum, Per Day, Per Hour
        public string? IR35Status { get; set; } // Inside IR35, Outside IR35, N/A
        public bool IsFTE { get; set; } // Full-Time Equivalent
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        // Navigation property for applications
        public virtual ICollection<Application>? Applications { get; set; }
    }
}
