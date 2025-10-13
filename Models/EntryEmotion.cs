namespace FinalCapstone.Models
{
    public class EntryEmotion
    {
        public int EntryId { get; set; }
        public int EmotionId { get; set; }

        public Entry Entry { get; set; }
        public Emotion Emotion { get; set; }
    }
}