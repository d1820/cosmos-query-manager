using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace CosmosManager.Tests.Integration
{
    /// <summary>
    /// Shared fixture for the CLI query harness. Connects to the local Cosmos emulator, provisions and
    /// seeds a deterministic HarnessDb/Items dataset, locates the built CosmosManager2019.exe, and runs
    /// .csql scripts through that exe so every scenario exercises the real translate-and-execute path.
    /// Skips (Available == false) when the emulator or the exe is unavailable.
    /// </summary>
    public class EmulatorHarnessFixture : IAsyncLifetime
    {
        public const string Endpoint = "http://localhost:8081";
        public const string Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public const string DatabaseId = "HarnessDb";
        public const string ContainerName = "Items";
        public const string ConnectionName = "Harness";

        /// <summary>Number of documents seeded into Items.</summary>
        public const int SeededCount = 5;

        public CosmosClient Client { get; private set; }
        public Container Container { get; private set; }
        public bool Available { get; private set; }
        public string InitError { get; private set; }

        private string _exePath;
        private string _workDir;
        private string _connectionsPath;

        public async Task InitializeAsync()
        {
            try
            {
                _exePath = LocateExe();
                if (_exePath == null)
                {
                    InitError = "CosmosManager2019.exe not found (build the solution first).";
                    return;
                }

                _workDir = Path.Combine(Path.GetTempPath(), "cmgr-harness-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(_workDir);

                Client = new CosmosClient(Endpoint, Key, new CosmosClientOptions
                {
                    ConnectionMode = ConnectionMode.Gateway,
                    LimitToEndpoint = true
                });

                await WaitForEmulatorAndProvisionAsync();
                await SeedAsync();
                WriteConnectionsFile();

                Available = true;
            }
            catch (Exception ex)
            {
                InitError = ex.Message;
                Available = false;
                Client?.Dispose();
                Client = null;
            }
        }

        public Task DisposeAsync()
        {
            try
            {
                Client?.GetDatabase(DatabaseId).DeleteAsync().GetAwaiter().GetResult();
            }
            catch
            {
                // best effort
            }
            Client?.Dispose();
            try
            {
                if (_workDir != null && Directory.Exists(_workDir))
                {
                    Directory.Delete(_workDir, true);
                }
            }
            catch
            {
                // best effort
            }
            return Task.CompletedTask;
        }

        /// <summary>Runs a .csql script through the exe and returns the parsed CLI result.</summary>
        public CliResult RunQuery(string csql)
        {
            var scriptPath = Path.Combine(_workDir, $"q_{Guid.NewGuid():N}.csql");
            var outputPath = Path.Combine(_workDir, $"q_{Guid.NewGuid():N}.cresult");
            File.WriteAllText(scriptPath, csql);

            var psi = new ProcessStartInfo
            {
                FileName = _exePath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            psi.ArgumentList.Add("exec");
            psi.ArgumentList.Add("--connections");
            psi.ArgumentList.Add(_connectionsPath);
            psi.ArgumentList.Add("--connectTo");
            psi.ArgumentList.Add(ConnectionName);
            psi.ArgumentList.Add("--script");
            psi.ArgumentList.Add(scriptPath);
            psi.ArgumentList.Add("--output");
            psi.ArgumentList.Add(outputPath);
            psi.ArgumentList.Add("--ignorePrompts");
            psi.ArgumentList.Add("--includeDocumentInOutput");

            using (var proc = Process.Start(psi))
            {
                proc.StandardOutput.ReadToEnd();
                proc.StandardError.ReadToEnd();
                proc.WaitForExit(60000);
                var output = File.Exists(outputPath) ? File.ReadAllText(outputPath) : "";
                return new CliResult { ExitCode = proc.HasExited ? proc.ExitCode : -1, Output = output };
            }
        }

        public async Task<JObject> ReadItemAsync(string id, string pk)
        {
            try
            {
                var response = await Container.ReadItemAsync<JObject>(id, new PartitionKey(pk));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        private async Task WaitForEmulatorAndProvisionAsync()
        {
            Exception last = null;
            for (var attempt = 0; attempt < 60; attempt++)
            {
                try
                {
                    var database = await Client.CreateDatabaseIfNotExistsAsync(DatabaseId);

                    // Recreate the container so every run starts from the known seed set.
                    try
                    {
                        await database.Database.GetContainer(ContainerName).DeleteContainerAsync();
                    }
                    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                    }

                    var container = await database.Database.CreateContainerAsync(ContainerName, "/pk");
                    Container = container.Container;
                    return;
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.ServiceUnavailable || (int)ex.StatusCode == 408)
                {
                    last = ex; // emulator still warming up
                }
                catch (HttpRequestException ex)
                {
                    last = ex; // not listening yet
                }
                await Task.Delay(2000);
            }
            throw new TimeoutException($"Cosmos emulator not ready at {Endpoint}. Last: {last?.Message}");
        }

        private async Task SeedAsync()
        {
            foreach (var doc in SeedDocuments())
            {
                await Container.UpsertItemAsync(doc, new PartitionKey(doc.Value<string>("pk")));
            }
        }

        /// <summary>
        /// Deterministic seed set (5 docs) designed so each scenario has a known expected count.
        /// </summary>
        public static IEnumerable<JObject> SeedDocuments()
        {
            yield return JObject.Parse("{\"id\":\"1\",\"pk\":\"p1\",\"name\":\"Alpha\",\"category\":\"books\",\"price\":10.0,\"qty\":5,\"active\":true,\"tags\":[\"red\",\"sale\"],\"created\":\"2020-01-01T00:00:00Z\"}");
            yield return JObject.Parse("{\"id\":\"2\",\"pk\":\"p1\",\"name\":\"Beta\",\"category\":\"books\",\"price\":20.0,\"qty\":3,\"active\":false,\"tags\":[\"blue\"],\"created\":\"2020-06-15T00:00:00Z\"}");
            yield return JObject.Parse("{\"id\":\"3\",\"pk\":\"p2\",\"name\":\"Gamma\",\"category\":\"toys\",\"price\":5.0,\"qty\":10,\"active\":true,\"tags\":[\"red\",\"clearance\"],\"created\":\"2021-03-10T00:00:00Z\"}");
            yield return JObject.Parse("{\"id\":\"4\",\"pk\":\"p2\",\"name\":\"Delta\",\"category\":\"toys\",\"price\":15.0,\"qty\":0,\"active\":true,\"tags\":[\"green\"],\"created\":\"2021-12-31T00:00:00Z\"}");
            yield return JObject.Parse("{\"id\":\"5\",\"pk\":\"p3\",\"name\":\"Epsilon\",\"category\":\"food\",\"price\":7.5,\"qty\":8,\"active\":false,\"tags\":[],\"created\":\"2022-07-04T00:00:00Z\"}");
        }

        private void WriteConnectionsFile()
        {
            _connectionsPath = Path.Combine(_workDir, "connections.json");
            var connections = new JArray(new JObject
            {
                ["Name"] = ConnectionName,
                ["EndPointUrl"] = Endpoint,
                ["ConnectionKey"] = Key,
                ["Database"] = DatabaseId
            });
            File.WriteAllText(_connectionsPath, connections.ToString(Formatting.Indented));
        }

        private static string LocateExe()
        {
            var fromEnv = Environment.GetEnvironmentVariable("COSMOSMANAGER_EXE");
            if (!string.IsNullOrEmpty(fromEnv) && File.Exists(fromEnv))
            {
                return fromEnv;
            }

            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "CosmosQueryEditor.sln")))
            {
                dir = dir.Parent;
            }
            if (dir == null)
            {
                return null;
            }

            var configs = AppContext.BaseDirectory.Contains("Release")
                ? new[] { "Release", "Debug" }
                : new[] { "Debug", "Release" };
            foreach (var config in configs)
            {
                var candidate = Path.Combine(dir.FullName, "CosmosManager", "bin", config, "net10.0-windows", "CosmosManager2019.exe");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
            return null;
        }
    }
}
