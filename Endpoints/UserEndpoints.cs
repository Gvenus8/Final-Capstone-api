using FinalCapstone.Data;
using FinalCapstone.DTOs;
using FinalCapstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalCapstone.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        // Update user profile
        app.MapPut("/api/user/profile", async (
            [FromBody] UpdateProfileDto dto,
            ClaimsPrincipal claimsPrincipal,
            UserManager<User> userManager) =>
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound("User not found");
            }

            // Validate display name
            if (string.IsNullOrWhiteSpace(dto.DisplayName))
            {
                return Results.BadRequest("Display name cannot be empty");
            }

            // Update display name
            user.DisplayName = dto.DisplayName;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Results.BadRequest($"Failed to update profile: {errors}");
            }

            // Return updated user info
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                IsAdmin = user.IsAdmin
            };

            return Results.Ok(userDto);
        }).RequireAuthorization();

        // Get user profile (same as /auth/me but in user endpoints)
        app.MapGet("/api/user/profile", async (
            ClaimsPrincipal claimsPrincipal,
            UserManager<User> userManager) =>
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound("User not found");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                IsAdmin = user.IsAdmin
            };

            return Results.Ok(userDto);
        }).RequireAuthorization();
        app.MapDelete("/api/user/profile", async (
               ClaimsPrincipal claimsPrincipal,
               UserManager<User> userManager,
               SignInManager<User> signInManager) =>
           {
               var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
               if (userId == null)
               {
                   return Results.Unauthorized();
               }

               var user = await userManager.FindByIdAsync(userId);
               if (user == null)
               {
                   return Results.NotFound("User not found");
               }

               // Delete the user (cascade will delete entries and entry emotions)
               var result = await userManager.DeleteAsync(user);

               if (!result.Succeeded)
               {
                   var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                   return Results.BadRequest($"Failed to delete account: {errors}");
               }

               // Sign out the user
               await signInManager.SignOutAsync();

               return Results.NoContent();
           }).RequireAuthorization();
    }
}
    
