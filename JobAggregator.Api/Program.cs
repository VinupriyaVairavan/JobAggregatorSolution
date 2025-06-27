using Microsoft.EntityFrameworkCore;
using JobAggregator.Core.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQLite DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        builder => builder.WithOrigins("http://localhost:5000", "http://localhost:5001", "http://localhost:8001") // Blazor app URL
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .WithExposedHeaders("X-Total-Count"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorApp"); // Use the CORS policy

app.UseAuthorization();

app.MapControllers();

app.Run();