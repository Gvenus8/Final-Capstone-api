using FinalCapstone.Data;
using FinalCapstone.DTOs;
using Microsoft.EntityFrameworkCore;



namespace FinalCapstone.Endpoints;

public static class EmotionEndpoints
{
    public static void MapEmotionEndpoints(this WebApplication app)
    {
        app.MapGet("/api/emotions", async (FinalCapstoneDbContext db) =>
        {
            var emotions = await db.Emotions
                .OrderBy(e => e.EmotionName)
                .ToListAsync();

            var emotionDtos = emotions.Select(e => new EmotionDto
            {
                Id = e.Id,
                EmotionName = e.EmotionName
            }).ToList();

            return Results.Ok(emotionDtos);
        }).RequireAuthorization();
        
    }
}