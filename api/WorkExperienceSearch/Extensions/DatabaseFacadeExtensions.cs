using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Work_Experience_Search.Extensions;

public static class DatabaseFacadeExtensions
{
    public static bool SupportsILike(this DatabaseFacade database) =>
        database.ProviderName?.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) == true;
}
