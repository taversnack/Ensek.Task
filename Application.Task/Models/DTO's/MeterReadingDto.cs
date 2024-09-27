using System.Text.RegularExpressions;

namespace Application.Models.DTO_s
{
    public class MeterReadingDto
    {
        public string AccountId { get; set; }
        public string MeterReadingDateTime { get; set; }
        public string MeterReadValue { get; set; } // NNNNN format

        public bool IsValidMeterReadValue()
        {
            return Regex.IsMatch(MeterReadValue, @"^(?:[1-9][0-9]{0,4})$");
        }
    }
}
