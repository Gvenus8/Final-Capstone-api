namespace FinalCapstone.Models
{
    public class Emotion
        {
        public int Id { get; set; }
        public string EmotionName { get; set; } = string.Empty;

        public List<EntryEmotion> EntryEmotions { get; set; } = new List<EntryEmotion>();
    }
}