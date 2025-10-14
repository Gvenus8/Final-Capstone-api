namespace FinalCapstone.DTOs
{
    public class EntryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Recipient { get; set; }
        public DateTime CreatedAt { get; set; }
        public EntryTypeDto EntryType { get; set; }
        public List<EmotionDto> Emotions { get; set; } = new List<EmotionDto>();
    }
}
