using System.ComponentModel.DataAnnotations;

namespace FinalCapstone.Models
{
    public class EntryEmotion
    {
        public int EntryId { get; set; }
        public int EmotionId { get; set; }

        public Entry Entry { get; set; } = null!; 
        public Emotion Emotion { get; set; } = null!;
    }
}