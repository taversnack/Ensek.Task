using Application.Models;

using Microsoft.EntityFrameworkCore;

using OfficeOpenXml;

namespace Application.Data
{
    public class EnergyDbContext : DbContext
    {
        public EnergyDbContext(DbContextOptions<EnergyDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // Seed method to load data from the CSV file
        // Method to seed data from the Excel file
        public void SeedAccountsFromExcel(string filePath)
        {

            // Set the EPPlus license context for non-commercial use
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Check if the data is already seeded
            if (Accounts.Any())
            {
                return; // Data already exists
            }

            // Load the Excel file
            FileInfo fileInfo = new FileInfo(filePath);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                // Get the first worksheet in the Excel file
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                // Get the number of rows
                int rowCount = worksheet.Dimension.Rows;

                // Loop through the rows (starting from 2 because row 1 is the header)
                for (int row = 2; row <= rowCount; row++)
                {
                    // Create a new Account object and populate its properties from the Excel data
                    var account = new Account
                    {
                        AccountId = int.Parse(worksheet.Cells[row, 1].Text.Trim()),
                        FirstName = worksheet.Cells[row, 2].Text.Trim(),
                        LastName = worksheet.Cells[row, 3].Text.Trim(),
                        Details = worksheet.Cells[row, 4].Text.Trim()
                        // Add more properties if necessary
                    };

                    // Add the account to the DbSet
                    Accounts.Add(account);
                }

                // Save changes to the database
                SaveChanges();
            }
        }
    }
}
