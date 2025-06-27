
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobAggregator.Core.Data;
using JobAggregator.Core.Models;

namespace JobAggregator.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobPostingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JobPostingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetJobPostings(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? jobType = null,
            [FromQuery] string? jobMode = null,
            [FromQuery] bool onlyToday = false,
            [FromQuery] decimal? minSalary = null,
            [FromQuery] decimal? maxSalary = null,
            [FromQuery] string? salaryUnit = null,
            [FromQuery] string? ir35Status = null,
            [FromQuery] bool? isFTE = null,
            [FromQuery] string? dateAddedFilter = null)
        {
            var query = _context.JobPostings.AsQueryable();

            if (!string.IsNullOrEmpty(jobType))
            {
                query = query.Where(jp => jp.JobType == jobType);
            }

            if (!string.IsNullOrEmpty(jobMode))
            {
                query = query.Where(jp => jp.JobMode == jobMode);
            }

            if (onlyToday)
            {
                var today = DateTime.Today;
                query = query.Where(jp => jp.PublishedDate.Date == today);
            }

            if (minSalary.HasValue)
            {
                query = query.Where(jp => jp.Salary >= minSalary.Value);
            }

            if (maxSalary.HasValue)
            {
                query = query.Where(jp => jp.Salary <= maxSalary.Value);
            }

            if (!string.IsNullOrEmpty(salaryUnit))
            {
                query = query.Where(jp => jp.SalaryUnit == salaryUnit);
            }

            if (!string.IsNullOrEmpty(ir35Status))
            {
                query = query.Where(jp => jp.IR35Status == ir35Status);
            }

            if (isFTE.HasValue)
            {
                query = query.Where(jp => jp.IsFTE == isFTE.Value);
            }

            if (!string.IsNullOrEmpty(dateAddedFilter))
            {
                query = dateAddedFilter.ToLower() switch
                {
                    "hour" => query.Where(jp => jp.DateAdded >= DateTime.UtcNow.AddHours(-1)),
                    "day" => query.Where(jp => jp.DateAdded >= DateTime.UtcNow.AddDays(-1)),
                    "week" => query.Where(jp => jp.DateAdded >= DateTime.UtcNow.AddDays(-7)),
                    "month" => query.Where(jp => jp.DateAdded >= DateTime.UtcNow.AddMonths(-1)),
                    "year" => query.Where(jp => jp.DateAdded >= DateTime.UtcNow.AddYears(-1)),
                    _ => query,
                };
            }

            var totalCount = await query.CountAsync();
            Response.Headers.Add("X-Total-Count", totalCount.ToString());

            var jobPostings = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return jobPostings;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobPosting>> GetJobPosting(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);

            if (jobPosting == null)
            {
                return NotFound();
            }

            return jobPosting;
        }
    }
}
