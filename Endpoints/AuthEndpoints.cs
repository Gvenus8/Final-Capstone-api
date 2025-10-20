using FinalCapstone.DTOs;
using FinalCapstone.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalCapstone.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/register", async (
            [FromBody] RegistrationDto registration,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager) =>
        {
            var existingUser = await userManager.FindByEmailAsync(registration.Email);
            if (existingUser != null)
            {
                return Results.BadRequest("A user with this email already exists.");
            }

            var newUser = new User
            {
                UserName = registration.Email,
                Email = registration.Email,
                DisplayName = registration.DisplayName,
                IsAdmin = false 
            };
            var result = await userManager.CreateAsync(newUser, registration.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Results.BadRequest($"Registration failed: {errors}");
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
            await userManager.AddToRoleAsync(newUser, "User");

            await signInManager.SignInAsync(newUser, isPersistent: false);

            var userDto = new UserDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                DisplayName = newUser.DisplayName,
                IsAdmin = newUser.IsAdmin
            };

            return Results.Created($"/auth/me", userDto);
        });

        app.MapPost("/auth/login", async (
            [FromBody] LoginDto login,
            SignInManager<User> signInManager,
            UserManager<User> userManager) =>
        {
            var user = await userManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                return Results.Unauthorized();
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, login.Password, false);
            if (!result.Succeeded)
            {
                return Results.Unauthorized();
            }

            await signInManager.SignInAsync(user, isPersistent: true);

           
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                DisplayName = user.DisplayName,
                IsAdmin = user.IsAdmin
            };

            return Results.Ok(userDto);
        });
        
        app.MapPost("/auth/logout", async (SignInManager<User> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.NoContent();
        });

        app.MapGet("/auth/me", async (
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
                return Results.Unauthorized();
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
    }
}