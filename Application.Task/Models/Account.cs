using System.ComponentModel.DataAnnotations;

namespace Application.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Details { get; set; }
    }
}
