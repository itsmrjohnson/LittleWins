namespace LittleWins.IntegrationTests;

public abstract class IntegrationTest : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture;
    private ApiFactory _factory = null!;

    protected HttpClient Client { get; private set; } = null!;

    protected IntegrationTest(
        IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        await _fixture.ResetDatabaseAsync();

        _factory = new ApiFactory(_fixture);

        Client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();

        await _factory.DisposeAsync();
    }
}