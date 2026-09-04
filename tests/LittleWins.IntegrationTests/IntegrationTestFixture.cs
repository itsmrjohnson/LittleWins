using LittleWins.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Testcontainers.MsSql;

namespace LittleWins.IntegrationTests;

public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServer =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

    private Respawner _respawner = null!;

    public string ConnectionString =>
        _sqlServer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _sqlServer.StartAsync();

        var options =
            new DbContextOptionsBuilder<LittleWinsDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

        await using var dbContext =
            new LittleWinsDbContext(options);

        await dbContext.Database.MigrateAsync();

        await using var connection =
            new SqlConnection(ConnectionString);

        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer
            });
    }

    public async Task ResetDatabaseAsync()
    {
        await using var connection =
            new SqlConnection(ConnectionString);

        await connection.OpenAsync();

        await _respawner.ResetAsync(connection);
    }

    public async Task DisposeAsync()
    {
        await _sqlServer.DisposeAsync();
    }
}