using System.Diagnostics;

var repoRoot = FindRepositoryRoot();

var services = new[]
{
    new ServiceDefinition("gateway", @"WorkForceHub.Gateway\WorkForceHub.Gateway.csproj"),
    new ServiceDefinition("account-command", @"AccountService\Command\Api\AccountService.Command.Api.csproj"),
    new ServiceDefinition("account-query", @"AccountService\Query\Api\AccountService.Query.Api.csproj"),
    new ServiceDefinition("profile-command", @"ProfileService\Command\Api\ProfileService.Command.Api.csproj"),
    new ServiceDefinition("profile-query", @"ProfileService\Query\Api\ProfileService.Query.Api.csproj"),
    new ServiceDefinition("time-command", @"TimeService\Command\Api\TimeService.Command.Api.csproj"),
    new ServiceDefinition("time-query", @"TimeService\Query\Api\TimeService.Query.Api.csproj"),
    new ServiceDefinition("evolution-command", @"EvolutionService\Command\Api\EvolutionService.Command.Api.csproj"),
    new ServiceDefinition("evolution-query", @"EvolutionService\Query\Api\EvolutionService.Query.Api.csproj"),
    new ServiceDefinition("media", @"MediaService\Api\MediaService.Api.csproj")
};

var runningProcesses = new List<Process>();
var shutdownRequested = 0;
var appLifetime = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    RequestShutdown("Ctrl+C received.");
};

AppDomain.CurrentDomain.ProcessExit += (_, _) => RequestShutdown("Process exit requested.");

Console.WriteLine("Starting WorkForceHub services...");
Console.WriteLine("Infrastructure still needs to be running separately, for example with 'make up-min'.");

foreach (var service in services)
{
    var process = StartService(service, repoRoot);
    runningProcesses.Add(process);
}

var exitWatchers = runningProcesses.Select(process => WatchForUnexpectedExitAsync(process, appLifetime)).ToArray();

await appLifetime.Task;
await StopAllAsync(runningProcesses);
await Task.WhenAll(exitWatchers);

return;

void RequestShutdown(string reason)
{
    if (Interlocked.Exchange(ref shutdownRequested, 1) == 1)
    {
        return;
    }

    Console.WriteLine(reason);
    appLifetime.TrySetResult();
}

async Task WatchForUnexpectedExitAsync(Process process, TaskCompletionSource lifetime)
{
    await process.WaitForExitAsync();

    if (Volatile.Read(ref shutdownRequested) == 1)
    {
        return;
    }

    var serviceName = process.StartInfo.Environment["WORKFORCEHUB_SERVICE_NAME"] ?? process.StartInfo.FileName;
    Console.WriteLine($"[{serviceName}] exited with code {process.ExitCode}.");
    lifetime.TrySetResult();
}

static Process StartService(ServiceDefinition service, string repoRootPath)
{
    var projectPath = Path.Combine(repoRootPath, service.ProjectPath);
    if (!File.Exists(projectPath))
    {
        throw new FileNotFoundException($"Project file not found for service '{service.Name}'.", projectPath);
    }

    var startInfo = new ProcessStartInfo
    {
        FileName = "dotnet",
        Arguments = $"run --project \"{projectPath}\" --launch-profile http",
        WorkingDirectory = repoRootPath,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true
    };

    startInfo.Environment["WORKFORCEHUB_SERVICE_NAME"] = service.Name;

    var process = new Process
    {
        StartInfo = startInfo,
        EnableRaisingEvents = true
    };

    process.OutputDataReceived += (_, args) =>
    {
        if (!string.IsNullOrWhiteSpace(args.Data))
        {
            Console.WriteLine($"[{service.Name}] {args.Data}");
        }
    };

    process.ErrorDataReceived += (_, args) =>
    {
        if (!string.IsNullOrWhiteSpace(args.Data))
        {
            Console.Error.WriteLine($"[{service.Name}] {args.Data}");
        }
    };

    if (!process.Start())
    {
        throw new InvalidOperationException($"Failed to start service '{service.Name}'.");
    }

    process.BeginOutputReadLine();
    process.BeginErrorReadLine();

    Console.WriteLine($"[{service.Name}] starting...");
    return process;
}

static async Task StopAllAsync(IEnumerable<Process> processes)
{
    foreach (var process in processes.Where(process => !process.HasExited))
    {
        try
        {
            process.Kill(entireProcessTree: true);
        }
        catch (InvalidOperationException)
        {
        }
    }

    foreach (var process in processes)
    {
        try
        {
            await process.WaitForExitAsync();
        }
        catch (InvalidOperationException)
        {
        }
    }
}

static string FindRepositoryRoot()
{
    var directory = new DirectoryInfo(AppContext.BaseDirectory);

    while (directory is not null)
    {
        if (directory.GetFiles("WorkForceHub.sln").Length > 0)
        {
            return directory.FullName;
        }

        directory = directory.Parent;
    }

    throw new InvalidOperationException("Unable to locate the repository root containing WorkForceHub.sln.");
}

internal sealed record ServiceDefinition(string Name, string ProjectPath);
