using System.ComponentModel.DataAnnotations;
namespace FinalCapstone.Models
{
    public class Entry
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Content { get; set; } = string.Empty;

        public string Recipient { get; set; } = string.Empty;
        public int EntryTypeId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

   
   public User User { get; set; } = null!;
        public EntryType EntryType { get; set; } = null!;
        public List<EntryEmotion> EntryEmotions { get; set; } = new List<EntryEmotion>();
    }
}