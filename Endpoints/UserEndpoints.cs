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

            if (string.IsNullOrWhiteSpace(dto.DisplayName))
            {
                return Results.BadRequest("Display name cannot be empty");
            }

            user.DisplayName = dto.DisplayName;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Results.BadRequest($"Failed to update profile: {errors}");
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
                Email = user.Email ?? string.Empty,
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
               var result = await userManager.DeleteAsync(user);

               if (!result.Succeeded)
               {
                   var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                   return Results.BadRequest($"Failed to delete account: {errors}");
               }

               await signInManager.SignOutAsync();

               return Results.NoContent();
           }).RequireAuthorization();
    }
}
    
