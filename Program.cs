using FinalCapstone.Data;
using FinalCapstone.Endpoints;
using FinalCapstone.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<FinalCapstoneDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CapstoneDbConnectionString"))
);


builder.Services
    .AddIdentity<User, IdentityRole>(options =>
    {
       
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
    })
    .AddEntityFrameworkStores<FinalCapstoneDbContext>();




builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "FinalCapstoneAuth";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.SlidingExpiration = true;


    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); 
    });
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

var app = builder.Build();
await DbInitializer.Initialize(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseCors("AllowAll");


app.UseAuthentication();
app.UseAuthorization();


app.MapAuthEndpoints();
app.MapEntryEndpoints();
app.MapEntryTypeEndpoints();
app.MapEmotionEndpoints();
app.MapUserEndpoints();
app.MapAdminEndpoints();

app.Run();