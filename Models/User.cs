using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FinalCapstone.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string DisplayName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public List<Entry> Entries { get; set; } = new List<Entry>();
    }
}