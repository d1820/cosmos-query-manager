using Xunit;

namespace CosmosManager.Tests.Integration
{
    public abstract class HarnessTestBase
    {
        protected readonly EmulatorHarnessFixture Fixture;

        protected HarnessTestBase(EmulatorHarnessFixture fixture)
        {
            Fixture = fixture;
        }

        protected void SkipIfUnavailable()
        {
            Skip.IfNot(Fixture.Available, $"Cosmos emulator or exe unavailable. {Fixture.InitError}");
        }
    }
}
