using Microsoft.Extensions.Configuration;
using Npgsql;

namespace UserManagement.Models;

internal static class DatabaseConnectionStringResolver
{
    private const string DefaultConnectionString =
        "Host=db;Port=5432;Database=user_db;Username=myuser;Password=mypassword";

    public static string Resolve(IConfiguration configuration)
    {
        var rawConnection = configuration.GetConnectionString("UserDatabase")
            ?? configuration["POSTGRES_URI"]
            ?? configuration["ConnectionStrings__UserDatabase"]
            ?? DefaultConnectionString;

        return Normalize(rawConnection);
    }

    private static string Normalize(string rawConnection)
    {
        if (string.IsNullOrWhiteSpace(rawConnection))
        {
            throw new InvalidOperationException("A PostgreSQL connection string is required.");
        }

        if (!rawConnection.Contains('=') && Uri.TryCreate(rawConnection, UriKind.Absolute, out var uri))
        {
            return BuildFromUri(uri).ConnectionString;
        }

        var builder = new NpgsqlConnectionStringBuilder(rawConnection);
        return builder.ConnectionString;
    }

    private static NpgsqlConnectionStringBuilder BuildFromUri(Uri uri)
    {
        if (string.IsNullOrWhiteSpace(uri.Host))
        {
            throw new InvalidOperationException("The PostgreSQL URI must include a host.");
        }

        var connectionBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.Trim('/'),
            SslMode = SslMode.Prefer
        };

        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            var parts = uri.UserInfo.Split(':', 2);
            connectionBuilder.Username = Uri.UnescapeDataString(parts[0]);
            if (parts.Length > 1)
            {
                connectionBuilder.Password = Uri.UnescapeDataString(parts[1]);
            }
        }

        if (!string.IsNullOrEmpty(uri.Query))
        {
            var query = uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in query)
            {
                var parts = pair.Split('=', 2);
                if (parts.Length == 2 && !string.IsNullOrEmpty(parts[0]))
                {
                    var key = Uri.UnescapeDataString(parts[0]);
                    var value = Uri.UnescapeDataString(parts[1]);
                    connectionBuilder[key] = value;
                }
            }
        }

        return connectionBuilder;
    }
}
