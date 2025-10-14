using Microsoft.AspNetCore.Identity;

namespace FinalCapstone.Models
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; }
        public bool IsAdmin { get; set; } = false;

        // Navigation property
        public List<Entry> Entries { get; set; } = new List<Entry>();
    }
}