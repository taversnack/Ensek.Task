using Application.Models.DTO_s;
using Application.Repositories.Interfaces;
using CsvHelper;
using System.Globalization;

namespace Application.Repositories.Implementations
{
    public class CsvRepository : ICsvRepository
    {
        public async Task<IEnumerable<MeterReadingDto>> ParseMeterReadingsAsync(Stream csvStream)
        {
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = await System.Threading.Tasks.Task.Run(() => csv.GetRecords<MeterReadingDto>().ToList());
            return records;
        }
    }
}
