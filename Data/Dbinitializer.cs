
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

            // Ensure database is created
            await db.Database.MigrateAsync();

            // Seed roles
            await SeedRoles(roleManager);

            // Seed entry types
            await SeedEntryTypes(db);

            // Seed emotions
            await SeedEmotions(db);

            // Seed test user
            await SeedTestUser(userManager, db);
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
        // Check if entry types already exist
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
        // Check if emotions already exist
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

    private static async Task SeedTestUser(UserManager<User> userManager, FinalCapstoneDbContext db)
    {
        // Check if test user already exists
        var testUserExists = await userManager.FindByEmailAsync("test@example.com");
        if (testUserExists != null)
        {
            return;
        }

        var testUser = new User
        {
            Email = "test@example.com",
            UserName = "test@example.com",
            DisplayName = "Test User",
            IsAdmin = false,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(testUser, "TestPassword123");

        if (result.Succeeded)
        {
            // Assign User role
            await userManager.AddToRoleAsync(testUser, "User");
        }
    }
}