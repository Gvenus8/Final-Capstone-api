using System.ComponentModel.DataAnnotations;

namespace FinalCapstone.Models
{
    public class EntryType
    {
        public int Id { get; set; }
        [Required]
        public string TypeName { get; set; } = string.Empty;

        public List<Entry> Entries { get; set; } = new List<Entry>();
    }
}