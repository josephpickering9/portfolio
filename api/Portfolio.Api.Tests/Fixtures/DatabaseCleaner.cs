using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Services;

namespace Portfolio.Api.Tests.Fixtures;

public static class DatabaseCleaner
{
    public static Task TruncateAllTablesAsync(this Database context)
    {
        var tables = context.Model.GetEntityTypes()
            .Select(entityType => entityType.GetTableName())
            .OfType<string>()
            .Distinct()
            .Select(table => $"\"{table}\"");

        return context.Database.ExecuteSqlRawAsync($"TRUNCATE {string.Join(", ", tables)} CASCADE");
    }
}
