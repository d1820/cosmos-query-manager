using CosmosManager.Domain;
using CosmosManager.Managers;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Xunit;

namespace CosmosManager.Tests.Unit
{
    /// <summary>
    /// Pure (no network) tests for the emulator-vs-production CosmosClient configuration logic.
    /// </summary>
    public class ClientConnectionManagerTests
    {
        [Fact]
        public void BuildClientOptions_HttpLocalhost_UsesGatewayWithoutCertCallback()
        {
            var connection = new Connection { EndPointUrl = "http://localhost:8081", ConnectionKey = "k" };

            ClientConnectionManager.IsEmulator(connection).Should().BeTrue();

            var options = ClientConnectionManager.BuildClientOptions(connection);
            options.Should().NotBeNull();
            options.ConnectionMode.Should().Be(ConnectionMode.Gateway);
            options.LimitToEndpoint.Should().BeTrue();
            options.ServerCertificateCustomValidationCallback.Should().BeNull();
        }

        [Fact]
        public void BuildClientOptions_HttpsLocalhost_SetsCertCallbackForSelfSignedEmulator()
        {
            var connection = new Connection { EndPointUrl = "https://localhost:8081", ConnectionKey = "k" };

            ClientConnectionManager.IsEmulator(connection).Should().BeTrue();

            var options = ClientConnectionManager.BuildClientOptions(connection);
            options.Should().NotBeNull();
            options.ConnectionMode.Should().Be(ConnectionMode.Gateway);
            options.ServerCertificateCustomValidationCallback.Should().NotBeNull();
        }

        [Fact]
        public void BuildClientOptions_ProductionEndpoint_ReturnsNull()
        {
            var connection = new Connection { EndPointUrl = "https://myaccount.documents.azure.com:443/", ConnectionKey = "k" };

            ClientConnectionManager.IsEmulator(connection).Should().BeFalse();
            ClientConnectionManager.BuildClientOptions(connection).Should().BeNull();
        }
    }
}
