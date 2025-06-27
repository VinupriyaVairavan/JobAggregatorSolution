
using System;
using System.ComponentModel.DataAnnotations;

namespace JobAggregator.Core.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }
        public int JobPostingId { get; set; }
        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
        public string? Status { get; set; } // e.g., Applied, Interviewing, Rejected, Hired
        public string? Notes { get; set; }

        // Navigation property to the JobPosting
        public virtual JobPosting? JobPosting { get; set; }
    }
}
