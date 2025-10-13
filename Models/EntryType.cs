namespace FinalCapstone.Models
{
    public class EntryType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }

        public List<Entry> Entries { get; set; } = new List<Entry>();
    }
}