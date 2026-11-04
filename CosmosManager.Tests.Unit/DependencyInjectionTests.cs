using FluentAssertions;
using Xunit;

namespace CosmosManager.Tests.Unit
{
    /// <summary>
    /// Headless verification that the command-line DI container is fully resolvable. This guards the
    /// SimpleInjector wiring (e.g. the concrete-type resolution / diagnostic-suppression changes
    /// required by SimpleInjector 5.x) without launching the GUI.
    /// </summary>
    public class DependencyInjectionTests
    {
        [Fact]
        public void CommandlineContainer_Verifies()
        {
            var container = Program.Bootstrap(bootCommandLine: true);

            container.Invoking(c => c.Verify()).Should().NotThrow();
        }
    }
}
