namespace Application.Models
{
    public class MeterReadingResult
    {
        public int SuccessfulReadings { get; }
        public int FailedReadings { get; }
        public int InvalidReadings { get; }
        public int AccountDoesNotExists { get; }
        public int DuplicateEntry { get; }

        public MeterReadingResult(int successfulReadings, int failedReadings, int invalidReadings, int accounts, int duplicate)
        {
            SuccessfulReadings = successfulReadings;
            FailedReadings = failedReadings;
            InvalidReadings = invalidReadings;
            AccountDoesNotExists = accounts;
            DuplicateEntry = duplicate;
        }
    }
}
