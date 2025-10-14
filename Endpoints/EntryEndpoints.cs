using FinalCapstone.Data;
using FinalCapstone.DTOs;
using FinalCapstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinalCapstone.Endpoints;

public static class EntryEndpoints
{
    public static void MapEntryEndpoints(this WebApplication app)
    {
        // Get all entries for current user (paginated)
        app.MapGet("/api/entries", async (
            ClaimsPrincipal user,
            FinalCapstoneDbContext db,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            // Get total count for pagination
            var totalCount = await db.Entries
                .Where(e => e.UserId == userId)
                .CountAsync();

            // Get paginated entries
            var entries = await db.Entries
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(e => e.EntryType)
                .Include(e => e.EntryEmotions)
                .ThenInclude(ee => ee.Emotion)
                .ToListAsync();

            // Map to DTOs
            var entryDtos = entries.Select(e => new EntryDto
            {
                Id = e.Id,
                Title = e.Title,
                Content = e.Content,
                Recipient = e.Recipient,
                CreatedAt = e.CreatedAt,
                EntryType = new EntryTypeDto
                {
                    Id = e.EntryType.Id,
                    TypeName = e.EntryType.TypeName
                },
                Emotions = e.EntryEmotions.Select(ee => new EmotionDto
                {
                    Id = ee.Emotion.Id,
                    EmotionName = ee.Emotion.EmotionName
                }).ToList()
            }).ToList();

            var response = new PaginatedResponse<EntryDto>
            {
                Data = entryDtos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Results.Ok(response);
        }).RequireAuthorization();

        // Get single entry by ID
        app.MapGet("/api/entries/{id}", async (
            int id,
            ClaimsPrincipal user,
            FinalCapstoneDbContext db) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var entry = await db.Entries
                .Include(e => e.EntryType)
                .Include(e => e.EntryEmotions)
                .ThenInclude(ee => ee.Emotion)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entry == null)
            {
                return Results.NotFound();
            }

            // Check if user owns this entry
            if (entry.UserId != userId)
            {
                return Results.Forbid();
            }

            var entryDto = new EntryDto
            {
                Id = entry.Id,
                Title = entry.Title,
                Content = entry.Content,
                Recipient = entry.Recipient,
                CreatedAt = entry.CreatedAt,
                EntryType = new EntryTypeDto
                {
                    Id = entry.EntryType.Id,
                    TypeName = entry.EntryType.TypeName
                },
                Emotions = entry.EntryEmotions.Select(ee => new EmotionDto
                {
                    Id = ee.Emotion.Id,
                    EmotionName = ee.Emotion.EmotionName
                }).ToList()
            };

            return Results.Ok(entryDto);
        }).RequireAuthorization();

        // Create new entry
        app.MapPost("/api/entries", async (
            [FromBody] CreateEntryDto dto,
            ClaimsPrincipal user,
            FinalCapstoneDbContext db) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.Title) ||
                string.IsNullOrWhiteSpace(dto.Content) ||
                string.IsNullOrWhiteSpace(dto.Recipient) ||
                dto.EntryTypeId <= 0 ||
                dto.EmotionIds == null || dto.EmotionIds.Count == 0)
            {
                return Results.BadRequest("All fields are required, including at least one emotion.");
            }

            // Create entry
            var entry = new Entry
            {
                UserId = userId,
                Title = dto.Title,
                Content = dto.Content,
                Recipient = dto.Recipient,
                EntryTypeId = dto.EntryTypeId,
                CreatedAt = DateTime.UtcNow
            };

            db.Entries.Add(entry);
            await db.SaveChangesAsync();

            // Add emotions
            foreach (var emotionId in dto.EmotionIds)
            {
                var entryEmotion = new EntryEmotion
                {
                    EntryId = entry.Id,
                    EmotionId = emotionId
                };
                db.EntryEmotions.Add(entryEmotion);
            }

            await db.SaveChangesAsync();

            // Fetch the created entry with all related data
            var createdEntry = await db.Entries
                .Include(e => e.EntryType)
                .Include(e => e.EntryEmotions)
                .ThenInclude(ee => ee.Emotion)
                .FirstOrDefaultAsync(e => e.Id == entry.Id);

            var entryDto = new EntryDto
            {
                Id = createdEntry.Id,
                Title = createdEntry.Title,
                Content = createdEntry.Content,
                Recipient = createdEntry.Recipient,
                CreatedAt = createdEntry.CreatedAt,
                EntryType = new EntryTypeDto
                {
                    Id = createdEntry.EntryType.Id,
                    TypeName = createdEntry.EntryType.TypeName
                },
                Emotions = createdEntry.EntryEmotions.Select(ee => new EmotionDto
                {
                    Id = ee.Emotion.Id,
                    EmotionName = ee.Emotion.EmotionName
                }).ToList()
            };

            return Results.Created($"/api/entries/{entry.Id}", entryDto);
        }).RequireAuthorization();

        // Update entry
        app.MapPut("/api/entries/{id}", async (
            int id,
            [FromBody] UpdateEntryDto dto,
            ClaimsPrincipal user,
            FinalCapstoneDbContext db) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var entry = await db.Entries.FirstOrDefaultAsync(e => e.Id == id);
            if (entry == null)
            {
                return Results.NotFound();
            }

            // Check if user owns this entry
            if (entry.UserId != userId)
            {
                return Results.Forbid();
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.Title) ||
                string.IsNullOrWhiteSpace(dto.Content) ||
                string.IsNullOrWhiteSpace(dto.Recipient) ||
                dto.EntryTypeId <= 0 ||
                dto.EmotionIds == null || dto.EmotionIds.Count == 0)
            {
                return Results.BadRequest("All fields are required, including at least one emotion.");
            }

            // Update entry fields
            entry.Title = dto.Title;
            entry.Content = dto.Content;
            entry.Recipient = dto.Recipient;
            entry.EntryTypeId = dto.EntryTypeId;

            // Remove old emotions
            var oldEmotions = db.EntryEmotions.Where(ee => ee.EntryId == id);
            db.EntryEmotions.RemoveRange(oldEmotions);

            // Add new emotions
            foreach (var emotionId in dto.EmotionIds)
            {
                var entryEmotion = new EntryEmotion
                {
                    EntryId = entry.Id,
                    EmotionId = emotionId
                };
                db.EntryEmotions.Add(entryEmotion);
            }

            await db.SaveChangesAsync();

            // Fetch updated entry
            var updatedEntry = await db.Entries
                .Include(e => e.EntryType)
                .Include(e => e.EntryEmotions)
                .ThenInclude(ee => ee.Emotion)
                .FirstOrDefaultAsync(e => e.Id == id);

            var entryDto = new EntryDto
            {
                Id = updatedEntry.Id,
                Title = updatedEntry.Title,
                Content = updatedEntry.Content,
                Recipient = updatedEntry.Recipient,
                CreatedAt = updatedEntry.CreatedAt,
                EntryType = new EntryTypeDto
                {
                    Id = updatedEntry.EntryType.Id,
                    TypeName = updatedEntry.EntryType.TypeName
                },
                Emotions = updatedEntry.EntryEmotions.Select(ee => new EmotionDto
                {
                    Id = ee.Emotion.Id,
                    EmotionName = ee.Emotion.EmotionName
                }).ToList()
            };

            return Results.Ok(entryDto);
        }).RequireAuthorization();

        // Delete entry
        app.MapDelete("/api/entries/{id}", async (
            int id,
            ClaimsPrincipal user,
            UserManager<User> userManager,
            FinalCapstoneDbContext db) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var entry = await db.Entries.FirstOrDefaultAsync(e => e.Id == id);
            if (entry == null)
            {
                return Results.NotFound();
            }

            // Check if user owns entry or is admin
            var currentUser = await userManager.FindByIdAsync(userId);
            var isAdmin = await userManager.IsInRoleAsync(currentUser, "Admin");

            if (entry.UserId != userId && !isAdmin)
            {
                return Results.Forbid();
            }

            // Delete entry (cascade deletes EntryEmotions)
            db.Entries.Remove(entry);
            await db.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization();
    }
}