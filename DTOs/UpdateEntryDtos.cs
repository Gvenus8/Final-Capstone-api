namespace FinalCapstone.DTOs
{
    public class UpdateEntryDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Recipient { get; set; }
        public int EntryTypeId { get; set; }
        public List<int> EmotionIds { get; set; } = new List<int>();
    }
}