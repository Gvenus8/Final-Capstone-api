namespace FinalCapstone.Models
{
    public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public String DisplayName { get; set; }
    public bool IsAdmin { get; set; } = false;

    public List<Entry> Entries { get; set; } = new List<Entry>();
}
}