using System.Collections.Generic;
using FoodAi.Api.Data;
using FoodAi.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FoodAi.Api.Tests.Integration;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        Environment.SetEnvironmentVariable("JWT_SECRET", "integration-test-secret-1234567890");
        Environment.SetEnvironmentVariable("SPOONACULAR_API_KEY", "test-key");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "integration-test-secret-1234567890",
                ["Jwt:Issuer"] = "FoodAi",
                ["Jwt:Audience"] = "FoodAi.Users",
                ["ConnectionStrings:Default"] = "DataSource=:memory:",
                ["Cors:Origin"] = "http://localhost:8080",
                ["Spoonacular:ApiKey"] = "test-key"
            };

            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<FoodAiDbContext>));
            services.RemoveAll(typeof(IEmailSender));

            _connection.Open();
            services.AddDbContext<FoodAiDbContext>(options => options.UseSqlite(_connection));
            services.AddSingleton<IEmailSender, TestEmailSender>();

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FoodAiDbContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection.Dispose();
        }

        base.Dispose(disposing);
    }
}
