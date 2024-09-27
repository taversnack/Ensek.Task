using Application.Models;
using Application.Models.DTO_s;

namespace Application.Repositories.Interfaces
{
    public interface IMeterReadingRepository
    {
        Task<bool> IsDuplicateReadingAsync(MeterReadingDto readingDto);
        Task<bool> AccountExistsAsync(string accountId);
        Task<bool> IsLatestReading(string accountId, DateTime date);
        System.Threading.Tasks.Task AddMeterReadingAsync(MeterReading reading);
        System.Threading.Tasks.Task SaveChangesAsync();
    }
}
