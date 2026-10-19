using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;
using Portfolio.Api.Services;

namespace Portfolio.Api.Tests.Fixtures;

public static class TestDatabase
{
    private const string PostgresImage = "postgres:17-alpine";
    private const string TemplateDatabase = "portfolio_test_template";
    private const string ConnectionStringEnvVar = "TEST_POSTGRES_CONNECTION";

    private static readonly SemaphoreSlim Gate = new(1, 1);

    private static PostgreSqlContainer? _container;
    private static string? _serverConnectionString;

    public static async Task<string> CreateAsync()
    {
        await EnsureTemplateAsync();

        var name = $"portfolio_test_{Guid.NewGuid():N}";
        await ExecuteAsync(ConnectionStringFor("postgres"), $"CREATE DATABASE \"{name}\" TEMPLATE \"{TemplateDatabase}\"");
        return ConnectionStringFor(name);
    }

    public static async Task DropAsync(string connectionString)
    {
        var name = new NpgsqlConnectionStringBuilder(connectionString).Database;
        NpgsqlConnection.ClearAllPools();
        await ExecuteAsync(ConnectionStringFor("postgres"), $"DROP DATABASE IF EXISTS \"{name}\" WITH (FORCE)");
    }

    private static async Task EnsureTemplateAsync()
    {
        if (_serverConnectionString is not null) return;

        await Gate.WaitAsync();
        try
        {
            if (_serverConnectionString is not null) return;

            var serverConnectionString = await StartServerAsync();
            await ExecuteAsync(WithDatabase(serverConnectionString, "postgres"), $"DROP DATABASE IF EXISTS \"{TemplateDatabase}\" WITH (FORCE)");
            await ExecuteAsync(WithDatabase(serverConnectionString, "postgres"), $"CREATE DATABASE \"{TemplateDatabase}\"");

            var options = new DbContextOptionsBuilder<Database>()
                .UseNpgsql(WithDatabase(serverConnectionString, TemplateDatabase))
                .Options;
            await using (var context = new Database(options))
            {
                await context.Database.MigrateAsync();
            }

            NpgsqlConnection.ClearAllPools();
            _serverConnectionString = serverConnectionString;
        }
        finally
        {
            Gate.Release();
        }
    }

    private static async Task<string> StartServerAsync()
    {
        var external = Environment.GetEnvironmentVariable(ConnectionStringEnvVar);
        if (!string.IsNullOrWhiteSpace(external)) return external;

        _container = new PostgreSqlBuilder(PostgresImage)
            .WithCommand("-c", "fsync=off", "-c", "synchronous_commit=off", "-c", "full_page_writes=off")
            .Build();
        await _container.StartAsync();
        return _container.GetConnectionString();
    }

    private static string ConnectionStringFor(string database) =>
        WithDatabase(_serverConnectionString ?? throw new InvalidOperationException("Test database server not started."), database);

    private static string WithDatabase(string connectionString, string database) =>
        new NpgsqlConnectionStringBuilder(connectionString) { Database = database }.ConnectionString;

    private static async Task ExecuteAsync(string connectionString, string sql)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync();
    }
}
