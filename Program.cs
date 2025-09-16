using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using mini_blog.Data;
using mini_blog.Entities;
using mini_blog.Helpers;
using mini_blog.Middleware;
using mini_blog.Services;
using mini_blog.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mini Blog API", Version = "v1" });
    c.SupportNonNullableReferenceTypes();
    c.MapType<object>(() => new OpenApiSchema { Type = "string" }); 
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddScoped<SlugService>();

builder.Services.AddDbContext<BlogDbContext>(options => 
    options.UseNpgsql(Environment.GetEnvironmentVariable("PostgresConnection")).UseSnakeCaseNamingConvention());

//Session
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Environment.GetEnvironmentVariable("RedisConnection");
    options.InstanceName = "MyAppSession:";
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "session";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

//cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.Name = "auth";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        
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


builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN"; // clients should send this header for state-changing requests
    options.Cookie.Name = ".MyApp.Antiforgery";
    options.Cookie.HttpOnly = false; // JavaScript must read the cookie to send token header in SPA scenarios
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAuthService, AuthService>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSession();

app.UseAuthentication();
app.UseMiddleware<CurrentUserMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();