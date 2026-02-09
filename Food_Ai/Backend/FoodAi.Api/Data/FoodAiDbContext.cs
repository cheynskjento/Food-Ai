using System.Text.Json;
using FoodAi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FoodAi.Api.Data;

public sealed class FoodAiDbContext : DbContext
{
    public FoodAiDbContext(DbContextOptions<FoodAiDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Preferences> Preferences => Set<Preferences>();
    public DbSet<ShoppingListItem> ShoppingListItems => Set<ShoppingListItem>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<RecipeCache> RecipeCaches => Set<RecipeCache>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var jsonOptions = new JsonSerializerOptions();
        var listConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, jsonOptions),
            v => string.IsNullOrWhiteSpace(v) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new List<string>());

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
            entity.Property(x => x.NormalizedEmail).HasMaxLength(320).IsRequired();
            entity.HasIndex(x => x.NormalizedEmail).IsUnique();
            entity.Property(x => x.PasswordHash).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();

            entity.HasOne(x => x.Preferences)
                .WithOne(x => x.User)
                .HasForeignKey<Preferences>(x => x.UserId);

            entity.HasMany(x => x.ShoppingListItems)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            entity.HasMany(x => x.PasswordResetTokens)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<Preferences>(entity =>
        {
            entity.HasKey(x => x.UserId);
            entity.Property(x => x.Dietary).HasConversion(listConverter);
            entity.Property(x => x.Allergies).HasConversion(listConverter);
            entity.Property(x => x.CuisinePreferences).HasConversion(listConverter);
            entity.Property(x => x.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<ShoppingListItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RecipeData).IsRequired();
            entity.Property(x => x.AddedAt).IsRequired();
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TokenHash).IsRequired();
            entity.Property(x => x.ExpiresAt).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<RecipeCache>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CacheKey).HasMaxLength(500).IsRequired();
            entity.Property(x => x.ResponseJson).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.ExpiresAt).IsRequired();
            entity.HasIndex(x => x.CacheKey).IsUnique();
        });
    }
}
