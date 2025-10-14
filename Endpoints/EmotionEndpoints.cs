using FinalCapstone.Data;
using FinalCapstone.DTOs;
using FinalCapstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinalCapstone.Endpoints;

public static class EmotionEndpoints
{
    public static void MapEmotionEndpoints(this WebApplication app)
    {
        // Get all emotions
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