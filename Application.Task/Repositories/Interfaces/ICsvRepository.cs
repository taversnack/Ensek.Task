using Application.Models.DTO_s;

namespace Application.Repositories.Interfaces
{
    public interface ICsvRepository
    {
        Task<IEnumerable<MeterReadingDto>> ParseMeterReadingsAsync(Stream csvStream);
    }
}
