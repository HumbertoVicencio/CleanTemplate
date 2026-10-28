namespace Kcl.Reso.CleanTemplate.IntegrationTests;

[CollectionDefinition(nameof(IntegrationTestCollection))]
public sealed class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebAppFactory>;
