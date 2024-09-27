using Application.Models;
using Application.Repositories.Interfaces;

namespace Application.Services
{
    public interface IMeterReadingService
    {
        Task<MeterReadingResult> ProcessMeterReadingsAsync(Stream csvStream);
    }

    public class MeterReadingService : IMeterReadingService
    {
        private readonly ICsvRepository _csvService;
        private readonly IMeterReadingRepository _repository;

        public MeterReadingService(ICsvRepository csvService, IMeterReadingRepository repository)
        {
            _csvService = csvService;
            _repository = repository;
        }

        public async Task<MeterReadingResult> ProcessMeterReadingsAsync(Stream csvStream)
        {
            var readings = await _csvService.ParseMeterReadingsAsync(csvStream);
            int successCount = 0;
            int failureCount = readings.Count();

            int invalidReadings = readings.Count();
            int accountDoesNotExists = 0;
            int duplicateEntry = 0;
            int notLatestEntry = 0;
            // filter invalid readings
            readings = readings.Where(x => x.IsValidMeterReadValue()).ToList();
            // filter non-existing account

            invalidReadings = invalidReadings - readings.Count();
            failureCount = failureCount - readings.Count();

            // filter duplicate readings

            foreach (var reading in readings)
            {
                if (!(await _repository.AccountExistsAsync(reading.AccountId)))
                {
                    failureCount++;
                    accountDoesNotExists++;
                    continue;
                }
                if
                    (await _repository.IsDuplicateReadingAsync(reading))
                {
                    failureCount++;
                    duplicateEntry++;
                    continue;
                }
                if (!await _repository.IsLatestReading(reading.AccountId, DateTime.ParseExact(reading.MeterReadingDateTime, "dd/MM/yyyy HH:mm", null)))
                {
                    failureCount++;
                    continue;
                }

                var meterReading = new MeterReading
                {
                    AccountId = reading.AccountId,
                    ReadingDate = DateTime.ParseExact(reading.MeterReadingDateTime, "dd/MM/yyyy HH:mm", null),
                    MeterReadValue = int.Parse(reading.MeterReadValue)
                };

                await _repository.AddMeterReadingAsync(meterReading);
                successCount++;
            }

            await _repository.SaveChangesAsync();

            return new MeterReadingResult(successCount, failureCount, invalidReadings, accountDoesNotExists, duplicateEntry);
        }
    }
}
