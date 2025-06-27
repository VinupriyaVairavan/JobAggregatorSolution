using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobAggregator.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddJobDetailsToJobPosting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IR35Status",
                table: "JobPostings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFTE",
                table: "JobPostings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                table: "JobPostings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalaryUnit",
                table: "JobPostings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IR35Status",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "IsFTE",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "SalaryUnit",
                table: "JobPostings");
        }
    }
}
