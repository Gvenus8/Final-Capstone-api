using FinalCapstone.Data;
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
        // Registration endpoint
        app.MapPost("/auth/register", async (
            [FromBody] RegistrationDto registration,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager) =>
        {
            // Check if email already exists
            var existingUser = await userManager.FindByEmailAsync(registration.Email);
            if (existingUser != null)
            {
                return Results.BadRequest("A user with this email already exists.");
            }

            // Create the User with IdentityUser properties
            var newUser = new User
            {
                UserName = registration.Email,
                Email = registration.Email,
                DisplayName = registration.DisplayName,
                IsAdmin = false // New users are not admins
            };

            // Create user (this hashes the password automatically)
            var result = await userManager.CreateAsync(newUser, registration.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Results.BadRequest($"Registration failed: {errors}");
            }

            // Ensure the "User" role exists
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Assign the "User" role to the new user
            await userManager.AddToRoleAsync(newUser, "User");

            // Sign in the user
            await signInManager.SignInAsync(newUser, isPersistent: false);

            // Return user info
            var userDto = new UserDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                DisplayName = newUser.DisplayName,
                IsAdmin = newUser.IsAdmin
            };

            return Results.Created($"/auth/me", userDto);
        });

        // Login endpoint
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

            // Sign in the user
            await signInManager.SignInAsync(user, isPersistent: true);

            // Return user info
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                IsAdmin = user.IsAdmin
            };

            return Results.Ok(userDto);
        });

        // Logout endpoint
        app.MapPost("/auth/logout", async (SignInManager<User> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.NoContent();
        });

        // Get current user info (requires authentication)
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
                Email = user.Email,
                DisplayName = user.DisplayName,
                IsAdmin = user.IsAdmin
            };

            return Results.Ok(userDto);
        }).RequireAuthorization();
    }
}