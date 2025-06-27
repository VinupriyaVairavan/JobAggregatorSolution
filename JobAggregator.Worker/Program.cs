using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.ServiceModel.Syndication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using JobAggregator.Core.Data;
using JobAggregator.Core.Models;

namespace JobAggregator.Worker
{
    public class Program
    {
        private static IConfiguration? _configuration;

        public static async Task Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite(connectionString);

            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                context.Database.Migrate(); // Apply any pending migrations

                var rssFeeds = _configuration.GetSection("RssFeeds").Get<string[]>();
                if (rssFeeds == null || rssFeeds.Length == 0)
                {
                    Console.WriteLine("No RSS feeds configured. Please add them to appsettings.json or environment variables.");
                    return;
                }

                var newJobs = await FetchAndSaveJobs(context, rssFeeds);

                if (newJobs.Any())
                {
                    await SendEmailNotification(newJobs);
                    Console.WriteLine($"Found and processed {newJobs.Count} new jobs.");
                }
                else
                {
                    Console.WriteLine("No new jobs found.");
                }
            }
        }

        private static async Task<List<JobPosting>> FetchAndSaveJobs(AppDbContext context, string[] rssFeeds)
        {
            var newJobs = new List<JobPosting>();
            foreach (var feedUrl in rssFeeds)
            {
                try
                {
                    using var reader = XmlReader.Create(feedUrl);
                    var feed = SyndicationFeed.Load(reader);

                    foreach (var item in feed.Items)
                    {
                        var link = item.Links.FirstOrDefault()?.Uri.ToString();
                        if (string.IsNullOrEmpty(link)) continue;

                        // Check for duplicate based on link
                        if (!await context.JobPostings.AnyAsync(j => j.Link == link))
                        {
                            var job = new JobPosting
                            {
                                Title = item.Title?.Text ?? string.Empty,
                                Link = link,
                                PublishedDate = item.PublishDate.DateTime,
                                Summary = item.Summary?.Text ?? string.Empty,
                                JobType = DetermineJobType(item.Title?.Text + " " + item.Summary?.Text),
                                JobMode = DetermineJobMode(item.Title?.Text + " " + item.Summary?.Text),
                                Salary = DetermineSalary(item.Title?.Text + " " + item.Summary?.Text),
                                SalaryUnit = DetermineSalaryUnit(item.Title?.Text + " " + item.Summary?.Text),
                                IR35Status = DetermineIR35Status(item.Title?.Text + " " + item.Summary?.Text),
                                IsFTE = DetermineIsFTE(item.Title?.Text + " " + item.Summary?.Text)
                            };
                            context.JobPostings.Add(job);
                            newJobs.Add(job);
                        }
                    }
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing feed {feedUrl}: {ex.Message}");
                }
            }
            return newJobs;
        }

        private static string DetermineJobType(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            text = text.ToLower();
            if (text.Contains("contract")) return "Contract";
            if (text.Contains("permanent")) return "Permanent";
            return string.Empty;
        }

        private static string DetermineJobMode(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            text = text.ToLower();
            if (text.Contains("remote")) return "Remote";
            if (text.Contains("hybrid")) return "Hybrid";
            if (text.Contains("onsite")) return "Onsite";
            return string.Empty;
        }

        private static decimal? DetermineSalary(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            text = text.ToLower().Replace(",", "");
            var match = System.Text.RegularExpressions.Regex.Match(text, @"\d+\.?\d*");
            if (match.Success && decimal.TryParse(match.Value, out var salary)) return salary;
            return null;
        }

        private static string? DetermineSalaryUnit(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            text = text.ToLower();
            if (text.Contains("per annum") || text.Contains("p.a.") || text.Contains("/year")) return "Per Annum";
            if (text.Contains("per day") || text.Contains("p.d.") || text.Contains("/day")) return "Per Day";
            if (text.Contains("per hour") || text.Contains("p.h.") || text.Contains("/hour")) return "Per Hour";
            return null;
        }

        private static string? DetermineIR35Status(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            text = text.ToLower();
            if (text.Contains("inside ir35")) return "Inside IR35";
            if (text.Contains("outside ir35")) return "Outside IR35";
            return "N/A";
        }

        private static bool DetermineIsFTE(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            text = text.ToLower();
            if (text.Contains("full-time") || text.Contains("fte")) return true;
            return false;
        }

        private static async Task SendEmailNotification(List<JobPosting> jobs)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var fromEmail = _configuration["SendGrid:FromEmail"];
            var toEmail = _configuration["SendGrid:ToEmail"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(toEmail))
            {
                Console.WriteLine("SendGrid configuration missing. Email not sent.");
                return;
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail);
            var to = new EmailAddress(toEmail);
            var subject = "New Job Postings Found!";
            var htmlContent = "<h1>New Job Postings</h1>";

            foreach (var job in jobs)
            {
                htmlContent += $"<h2><a href='{job.Link}'>{job.Title}</a></h2>";
                htmlContent += $"<p><b>Published:</b> {job.PublishedDate.ToShortDateString()}</p>";
                htmlContent += $"<p>{job.Summary}</p><hr>";
            }

            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Email sent successfully!");
            }
            else
            {
                Console.WriteLine($"Failed to send email. Status Code: {response.StatusCode}");
                Console.WriteLine(await response.Body.ReadAsStringAsync());
            }
        }
    }
}