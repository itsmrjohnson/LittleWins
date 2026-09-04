namespace LittleWins.IntegrationTests;

[CollectionDefinition("Integration tests")]
public sealed class IntegrationTestCollection
    : ICollectionFixture<IntegrationTestFixture>
{
}

