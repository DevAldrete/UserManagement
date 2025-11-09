using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagement.Models;

namespace UserManagement.Data;

internal static class DatabaseSeeder
{
    private static readonly User[] DefaultUsers =
    [
        new()
        {
            Id = Guid.Parse("8c48ce8e-9650-4dc5-ae2c-237635b38767"),
            FirstName = "Alex",
            LastName = "Johnson",
            Email = "alex.johnson@example.com",
            PhoneNumber = "+1-404-555-0100",
            Notes = "Champion of the customer success team.",
            IsActive = true,
            CreatedAt = new DateTimeOffset(2024, 2, 15, 14, 30, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 6, 2, 18, 45, 0, TimeSpan.Zero)
        },
        new()
        {
            Id = Guid.Parse("3f949f58-8733-4da2-91a4-0d85f001adcf"),
            FirstName = "Maria",
            LastName = "Lopez",
            Email = "maria.lopez@example.com",
            PhoneNumber = "+34-91-555-0123",
            Notes = "Prefers communication in Spanish and email follow-ups.",
            IsActive = true,
            CreatedAt = new DateTimeOffset(2024, 3, 1, 9, 15, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 7, 8, 10, 5, 0, TimeSpan.Zero)
        },
        new()
        {
            Id = Guid.Parse("42f91283-15f6-4c6c-b7ce-9635892fe887"),
            FirstName = "Noah",
            LastName = "Williams",
            Email = "noah.williams@example.com",
            PhoneNumber = null,
            Notes = "Working remotely across PST hours.",
            IsActive = false,
            CreatedAt = new DateTimeOffset(2023, 12, 11, 17, 5, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 4, 21, 12, 20, 0, TimeSpan.Zero)
        },
        new()
        {
            Id = Guid.Parse("10887f5c-1c36-4b73-9c29-3de7d8247391"),
            FirstName = "Priya",
            LastName = "Patel",
            Email = "priya.patel@example.com",
            PhoneNumber = "+91-22-555-0199",
            Notes = "On-boarding mentor for new hires.",
            IsActive = true,
            CreatedAt = new DateTimeOffset(2024, 5, 19, 8, 0, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 8, 14, 9, 42, 0, TimeSpan.Zero)
        },
        new()
        {
            Id = Guid.Parse("1c016a4a-6b02-4f82-afc9-08d8aa65877c"),
            FirstName = "Elias",
            LastName = "Nguyen",
            Email = "elias.nguyen@example.com",
            PhoneNumber = "+61-2-5555-8899",
            Notes = "Maintains the APAC partner accounts.",
            IsActive = true,
            CreatedAt = new DateTimeOffset(2024, 6, 27, 11, 10, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 9, 3, 15, 0, 0, TimeSpan.Zero)
        }
    ];

    public static async Task SeedAsync(UserManagementDbContext context, ILogger logger, CancellationToken cancellationToken = default)
    {
        var hasUsers = await context.Users.AnyAsync(cancellationToken);
        if (hasUsers)
        {
            logger.LogInformation("Skipping database seed; user records already exist.");
            return;
        }

        context.Users.AddRange(DefaultUsers);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {UserCount} user records", DefaultUsers.Length);
    }
}
