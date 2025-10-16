namespace FinalCapstone.DTOs
{
    public class EntryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public EntryTypeDto EntryType { get; set; } = null!;
        public List<EmotionDto> Emotions { get; set; } = new List<EmotionDto>();
    }
}
