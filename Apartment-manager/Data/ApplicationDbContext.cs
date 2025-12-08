using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;

// Namespace for your application's data layer
namespace MyWebApp.Data
{
    // The DbContext represents a session with the database and allows querying and saving data.
    // Inheriting from IdentityDbContext<TUser> is common for ASP.NET Core applications 
    // that use the built-in Identity system for user authentication and authorization.
    // If you don't use Identity, you can inherit directly from DbContext.
    public class ApplicationDbContext : IdentityDbContext
    {
        // Constructor that accepts DbContextOptions. This is how the database provider (e.g., SQL Server, SQLite)
        // and connection string are passed to the context, typically from the Program.cs or Startup.cs file.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet properties represent the collections (tables) in the database.
        // Replace 'TodoItem' with your actual model classes.
        public DbSet<TodoItem> TodoItems { get; set; } = default!;

        // Optionally, override OnModelCreating to configure the database schema, 
        // define relationships, constraints, or seed data.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Must call the base method for IdentityDbContext to ensure
            // the Identity tables (Users, Roles, etc.) are created correctly.
            base.OnModelCreating(builder);

            // Example: Configure a primary key or data seeding
            builder.Entity<TodoItem>().HasKey(t => t.Id);

            // Example: Seed initial data
            builder.Entity<TodoItem>().HasData(
                new TodoItem { Id = 1, Title = "Buy Milk", IsComplete = false },
                new TodoItem { Id = 2, Title = "Finish Project Proposal", IsComplete = true }
            );
        }
    }

    // --- Sample Model Definition (Typically in a separate Models folder/file) ---

    /// <summary>
    /// A simple model representing a task in a to-do list.
    /// </summary>
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        // public string? UserId { get; set; } // Optional: To link an item to a specific user
    }
}