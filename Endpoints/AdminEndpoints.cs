using FinalCapstone.Data;
using FinalCapstone.DTOs;
using FinalCapstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalCapstone.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/admin").RequireAuthorization(policy =>
            policy.RequireRole("Admin"));


        group.MapGet("/users", async (FinalCapstoneDbContext db) =>
        {
            var users = await db.Users
                .Select(u => new AdminUserViewDto
                {
                    Id = u.Id,
                    Email = u.Email ?? string.Empty,
                    DisplayName = u.DisplayName,
                    IsAdmin = u.IsAdmin,
                    EntryCount = u.Entries.Count()
                })
                .OrderBy(u => u.DisplayName)
                .ToListAsync();

            return Results.Ok(users);
        });

        group.MapGet("/users/{userId}", async (
            string userId,
            FinalCapstoneDbContext db) =>
        {
            var user = await db.Users
                .Where(u => u.Id == userId)
                .Select(u => new AdminUserDetailDto
                {
                    Id = u.Id,
                    Email = u.Email ?? string.Empty,
                    DisplayName = u.DisplayName,
                    IsAdmin = u.IsAdmin,
                    EntryCount = u.Entries.Count()
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Results.NotFound("User not found");
            }

            return Results.Ok(user);
        });


        group.MapDelete("/users/{userId}", async (
            string userId,
            UserManager<User> userManager) =>
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound("User not found");
            }

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Results.Ok(new { message = $"User {user.DisplayName} deleted successfully" });
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Results.BadRequest($"Failed to delete user: {errors}");
        });

        group.MapGet("/statistics", async (FinalCapstoneDbContext db) =>
          {
              var stats = new AdminStatisticsDto
              {
                  TotalUsers = await db.Users.CountAsync(),
                  TotalEntries = await db.Entries.CountAsync(),
                  MostUsedEntryTypes = await db.EntryTypes
                      .Select(et => new EntryTypeStatsDto
                      {
                          Type = et.TypeName,
                          Count = et.Entries.Count()
                      })
                      .OrderByDescending(x => x.Count)
                      .Take(5)
                      .ToListAsync()
              };

              return Results.Ok(stats);
          });
    }
}