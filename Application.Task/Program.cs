using Application.Data;
using Application.Repositories.Implementations;
using Application.Repositories.Interfaces;
using Application.Services;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Register the DbContext with dependency injection
builder.Services.AddDbContext<EnergyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMeterReadingService, MeterReadingService>();
builder.Services.AddScoped<MeterReadingService>();
builder.Services.AddScoped<ICsvRepository, CsvRepository>();
builder.Services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seed data when the application starts
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<EnergyDbContext>();

        // Call the Excel seeder with the file path to the Excel file
        dbContext.SeedAccountsFromExcel("Data/Test_Accounts.xlsx"); // Ensure correct file path
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}


app.Run();
