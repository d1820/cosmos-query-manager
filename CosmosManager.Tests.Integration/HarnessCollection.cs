using Xunit;

namespace CosmosManager.Tests.Integration
{
    /// <summary>
    /// Shares a single seeded <see cref="EmulatorHarnessFixture"/> across all scenario classes.
    /// Tests within a collection run sequentially, so write scenarios can arrange/cleanup safely.
    /// </summary>
    [CollectionDefinition("Emulator harness")]
    public class HarnessCollection : ICollectionFixture<EmulatorHarnessFixture>
    {
    }
}
