using Application.Data;
using Application.Models;
using Application.Models.DTO_s;
using Application.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Repositories.Implementations
{
    public class MeterReadingRepository : IMeterReadingRepository
    {
        private readonly ICsvRepository _csvRepository;
        private readonly EnergyDbContext _context;
        public MeterReadingRepository(ICsvRepository csvRepository, EnergyDbContext context)
        {
            _csvRepository = csvRepository;
            _context = context;
        }

        public Task<bool> AccountExistsAsync(string accountId)
        {
            return _context.Accounts.AnyAsync(x => x.AccountId == Convert.ToInt32(accountId));
        }

        public async System.Threading.Tasks.Task AddMeterReadingAsync(MeterReading reading)
        {
            await _context.MeterReadings.AddAsync(reading);
        }

        public async Task<bool> IsDuplicateReadingAsync(MeterReadingDto readingDto)
        {
            DateTime readingDate = DateTime.ParseExact(readingDto.MeterReadingDateTime, "dd/MM/yyyy HH:mm", null);
            return await _context.MeterReadings.AnyAsync(mr =>
              mr.AccountId == readingDto.AccountId &&
              mr.ReadingDate == readingDate &&
              mr.MeterReadValue == int.Parse(readingDto.MeterReadValue));
        }

        public async Task<bool> IsLatestReading(string accountId, DateTime date)
        {
            // Get the latest reading date from the database or other storage
            DateTime? latestReadingDate = await GetLatestReadingDateAsync(accountId);

            // If there is no previous reading, assume this is the latest
            if (!latestReadingDate.HasValue)
            {
                return true;
            }

            // Return true if the provided date is newer than the latest reading
            return date > latestReadingDate.Value;

        }
        public async Task<DateTime?> GetLatestReadingDateAsync(string accountId)
        {
            // Query the database for the latest meter reading for a given account
            var latestReading = await _context.MeterReadings
                .Where(r => r.AccountId == accountId)    // Filter by account ID
                .OrderByDescending(r => r.ReadingDate)   // Order by date, latest first
                .FirstOrDefaultAsync();                  // Get the first (most recent) reading

            // Return the date of the latest reading, or null if no reading exists
            return latestReading?.ReadingDate;
        }

        public async System.Threading.Tasks.Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
