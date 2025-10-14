
using Microsoft.AspNetCore.Identity;
namespace FinalCapstone.Models
{
    public class Entry
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Recipient { get; set; }
        public int EntryTypeId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

   
   public User User { get; set; }
   public EntryType EntryType { get; set; }
        public List<EntryEmotion> EntryEmotions { get; set; } = new List<EntryEmotion>();
    }
}