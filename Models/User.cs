using System.ComponentModel.DataAnnotations;

namespace FinalCapstone.Models

{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public String DisplayName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsAdmin { get; set; } = false;

        public List<Entry> Entries { get; set; } = new List<Entry>();
    }
}