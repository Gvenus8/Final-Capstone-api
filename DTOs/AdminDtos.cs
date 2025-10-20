namespace FinalCapstone.DTOs;

public class AdminUserViewDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public DateTime CreatedDate { get; set; }
    public int EntryCount { get; set; }
    public DateTime? LastEntryDate { get; set; }
}

public class AdminUserDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public DateTime CreatedDate { get; set; }
    public int EntryCount { get; set; }

}
public class AdminStatisticsDto
{
    public int TotalUsers { get; set; }
    public int TotalEntries { get; set; }
    public List<EntryTypeStatsDto> MostUsedEntryTypes { get; set; } = new(); // ✅ Change this
}
    
public class EntryTypeStatsDto
{
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
}

