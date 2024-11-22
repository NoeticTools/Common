using NoeticTools.Common.Tools;
using NoeticTools.Common.Tools.DotnetCli;
using NoeticTools.Git2SemVer.Core.IntegrationTests.Framework;


namespace NoeticTools.Git2SemVer.Core.IntegrationTests;

internal sealed class DotNetToolIntegrationTestContext : IDisposable
{
    private const int ConcurrentContextsLimit = 100;
    private static int _activeContexts;

    public DotNetToolIntegrationTestContext()
    {
        if (_activeContexts > ConcurrentContextsLimit)
        {
            Assert.Fail($"Exceeded number of active contexts limit of {ConcurrentContextsLimit}.");
        }
        _activeContexts++;

        Logger = new NUnitLogger(false);

        TestDirectory = TestDirectoryResource.Get();
        TestFolderName = TestDirectory.Name;
        Logger.LogInfo("Created test directory {0}.", TestDirectory.FullName);

        var processCli = new ProcessCli(Logger) { WorkingDirectory = TestDirectory.FullName };
        DotNetCli = new DotNetTool(processCli);
    }

    public DotNetTool DotNetCli { get; }

    public NUnitLogger Logger { get; }

    public DirectoryInfo TestDirectory { get; }

    public string TestFolderName { get; }

    public void Dispose()
    {
        _activeContexts--;
        TestDirectoryResource.Release();
    }
}