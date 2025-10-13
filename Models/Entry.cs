

namespace FinalCapstone.Models
{
    public class Entry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Recipient { get; set}
        public int EntryTypeId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
   
   public User User { get; set; }
   public EntryType EntryType { get; set; }
        public List<EntryEmotion> EntryEmotions { get; set; } = new List<EntryEmotion>();
    }
}