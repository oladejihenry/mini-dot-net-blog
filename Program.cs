using FluentValidation;
using Microsoft.EntityFrameworkCore;
using mini_blog.Data;
using mini_blog.Helpers;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddScoped<SlugService>();

builder.Services.AddDbContext<BlogDbContext>(options => 
    options.UseSqlServer(System.Environment.GetEnvironmentVariable("DefaultConnection")).UseSnakeCaseNamingConvention());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();