namespace Application.Models
{
    public class MeterReading
    {
        public int Id { get; set; }
        public string AccountId { get; set; } // Foreign key to Account
        public DateTime ReadingDate { get; set; }
        public int MeterReadValue { get; set; } // Must be in NNNNN format
    }
}
