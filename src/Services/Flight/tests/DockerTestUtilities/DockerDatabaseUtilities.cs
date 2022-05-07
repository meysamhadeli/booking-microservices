using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;

namespace Integration.Test.DockerTestUtilities;

public static class DockerDatabaseUtilities
{
    private const string ACCEPT_EULA = "Y";
    private const string SA_PASSWORD = "@Aa123456";
    private const string DB_CONTAINER_NAME = "sqldb";
    private static readonly ImageTag ImageTagForOs = new("mcr.microsoft.com/mssql/server", "2017-latest");

    public static async Task<int> EnsureDockerStartedAndGetPortPortAsync()
    {
        await DockerUtilities.CleanupRunningContainers(DB_CONTAINER_NAME);
        await DockerUtilities.CleanupRunningVolumes(DB_CONTAINER_NAME);

        var hosts = new Hosts().Discover();
        var docker = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

        // create container, if one doesn't already exist
        var existingContainer = docker?.GetContainers().FirstOrDefault(c => c.Name == DB_CONTAINER_NAME);

        if (existingContainer == null)
        {
            var container = new Builder().UseContainer()
                .WithName(DB_CONTAINER_NAME)
                .UseImage($"{ImageTagForOs.Image}:{ImageTagForOs.Tag}")
                .ExposePort(1433, 1433)
                .WithEnvironment(
                    $"SA_PASSWORD={SA_PASSWORD}",
                    $"ACCEPT_EULA={ACCEPT_EULA}")
                .WaitForPort("1433/tcp", 30000 /*30s*/)
                .Build();

            container.Start();

            await DockerUtilities.WaitUntilDatabaseAvailableAsync(GetSqlConnectionString());
        }

        return existingContainer.ToHostExposedEndpoint("1433/tcp").Port;
    }

    // SQL Server 2019 does not work on macOS + M1 chip. So we use SQL Edge as a workaround until SQL Server 2022 is GA.
    // See https://github.com/pdevito3/craftsman/issues/53 for details.
    private static ImageTag GetImageTagForOs()
    {
        var sqlServerImageTag = new ImageTag("mcr.microsoft.com/mssql/server", "2019-latest");
        var sqlEdgeImageTag = new ImageTag("mcr.microsoft.com/azure-sql-edge", "latest");
        return IsRunningOnMacOsArm64() ? sqlEdgeImageTag : sqlServerImageTag;
    }

    private static bool IsRunningOnMacOsArm64()
    {
        var isMacOs = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        var cpuArch = RuntimeInformation.ProcessArchitecture;
        return isMacOs && cpuArch == Architecture.Arm64;
    }

    public static string GetSqlConnectionString()
    {
        return DockerUtilities.GetSqlConnectionString();
    }

    private record ImageTag(string Image, string Tag);
}
