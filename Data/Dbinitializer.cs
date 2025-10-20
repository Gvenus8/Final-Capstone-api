using FinalCapstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinalCapstone.Data;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<FinalCapstoneDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await db.Database.MigrateAsync();

            await SeedRoles(roleManager);
            await SeedEntryTypes(db);
            await SeedEmotions(db);
            await SeedUsers(userManager, db);
        }
    }
    private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "Admin", "User" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
    private static async Task SeedEntryTypes(FinalCapstoneDbContext db)
    {
        if (db.EntryTypes.Any())
        {
            return;
        }
        var entryTypes = new List<EntryType>
        {
            new EntryType { TypeName = "Memory" },
            new EntryType { TypeName = "Letting Go" },
            new EntryType { TypeName = "Gratitude" },
            new EntryType { TypeName = "Reflection" },
            new EntryType { TypeName = "Inspiration" }
        };

        await db.EntryTypes.AddRangeAsync(entryTypes);
        await db.SaveChangesAsync();
    }
    private static async Task SeedEmotions(FinalCapstoneDbContext db)
    {
        if (db.Emotions.Any())
        {
            return;
        }
        var emotions = new List<Emotion>
        {
            new Emotion { EmotionName = "Happy" },
            new Emotion { EmotionName = "Sad" },
            new Emotion { EmotionName = "Angry" },
            new Emotion { EmotionName = "Peaceful" },
            new Emotion { EmotionName = "Grateful" },
            new Emotion { EmotionName = "Anxious" },
            new Emotion { EmotionName = "Hopeful" },
            new Emotion { EmotionName = "Overwhelmed" },
            new Emotion { EmotionName = "Loved" },
            new Emotion { EmotionName = "Lonely" }
        };

        await db.Emotions.AddRangeAsync(emotions);
        await db.SaveChangesAsync();
    }

    private static async Task SeedUsers(UserManager<User> userManager, FinalCapstoneDbContext db)
    {
        var usersToSeed = new[]
        {
        new {
            Email = "gjv@nss.com",
            DisplayName = "Gary Venus (ADMIN)",
            Password = "Admin123!",
            IsAdmin = true,
            Roles = new[] { "User", "Admin" }
        },
        new {
            Email = "testuser@example.com",
            DisplayName = "Test User",
            Password = "TestUser123!",
            IsAdmin = false,
            Roles = new[] { "User" }
        }
    };

        foreach (var userData in usersToSeed)
        {
            var existingUser = await userManager.FindByEmailAsync(userData.Email);
            if (existingUser != null)
            {
                continue; 
            }
            var user = new User
            {
                Email = userData.Email,
                UserName = userData.Email,
                DisplayName = userData.DisplayName,
                IsAdmin = userData.IsAdmin,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, userData.Password);
            if (result.Succeeded)
            {
                foreach (var role in userData.Roles)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user {userData.Email}: {errors}");
            }
        }
    }
}