using FinalCapstone.Data;
using FinalCapstone.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FinalCapstone.Endpoints;

public static class EntryTypeEndpoints
{
    public static void MapEntryTypeEndpoints(this WebApplication app)
    {
        app.MapGet("/api/entrytypes", async (FinalCapstoneDbContext db) =>
        {
            var entryTypes = await db.EntryTypes
                .OrderBy(et => et.TypeName)
                .ToListAsync();

            var entryTypeDtos = entryTypes.Select(et => new EntryTypeDto
            {
                Id = et.Id,
                TypeName = et.TypeName
            }).ToList();

            return Results.Ok(entryTypeDtos);
        });
    }
}
