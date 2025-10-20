namespace FinalCapstone.DTOs
{
    public class EmotionDto
    {
        public int Id { get; set; }
        public string EmotionName { get; set; }
    }
    public class EmotionStatDto
    {
        public string Emotion { get; set; }
        public int Count { get; set; }
    }
}