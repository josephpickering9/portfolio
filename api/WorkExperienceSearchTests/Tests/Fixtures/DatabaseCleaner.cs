using Microsoft.EntityFrameworkCore;
using Work_Experience_Search.Services;

namespace WorkExperienceSearchTests.Tests.Fixtures;

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
