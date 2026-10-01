using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace QLThuGomRac_UNETI02_TI17A1HN.Data;

/// <summary>
/// Resolve the current LocalDB pipe outside the VS debuggee. On affected Windows
/// installations, native LocalDB discovery inside the debugger fails with error 575.
/// The pipe is resolved on each launch because it changes when LocalDB restarts.
/// </summary>
public static class DevelopmentLocalDb
{
    public static string ResolveConnectionString(string connectionString)
    {
        var connection = new SqlConnectionStringBuilder(connectionString);
        const string prefix = @"(localdb)\";
        if (!connection.DataSource.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return connectionString;

        var instance = connection.DataSource[prefix.Length..];
        // Shared instances retain SqlClient's standard resolution behavior.
        if (instance.StartsWith('.')) return connectionString;

        var info = RunLocalDb("info", instance);
        var pipe = FindPipe(info) ?? FindRunningPipe(instance);
        if (pipe == null)
        {
            RunLocalDb("start", instance);
            info = RunLocalDb("info", instance);
            pipe = FindPipe(info) ?? FindRunningPipe(instance);
        }
        if (pipe == null)
            throw new InvalidOperationException($"Không lấy được đường dẫn kết nối LocalDB '{instance}'. Thông tin instance: {info}");

        connection.DataSource = pipe;
        return connection.ConnectionString;
    }

    private static string? FindRunningPipe(string instance)
    {
        if (instance.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) return null;
        var log = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Microsoft", "Microsoft SQL Server Local DB", "Instances", instance, "error.log");
        if (!File.Exists(log)) return null;
        using var stream = new FileStream(log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using var reader = new StreamReader(stream);
        var matches = Regex.Matches(reader.ReadToEnd(), @"\\\\\.\\pipe\\LOCALDB#[a-zA-Z0-9]+\\tsql\\query", RegexOptions.IgnoreCase);
        if (matches.Count == 0) return null;
        var address = matches[^1].Value;
        // Confirm the pipe exists now; never use a stale address from a stopped server.
        return File.Exists(address) ? "np:" + address : null;
    }

    private static string? FindPipe(string output)
    {
        // Match the address, not the localized label from sqllocaldb.
        var match = Regex.Match(output, @"np:\\\\\.\\pipe\\[^\r\n]+", RegexOptions.IgnoreCase);
        return match.Success ? match.Value.Trim() : null;
    }

    private static string RunLocalDb(string command, string instance)
    {
        var startInfo = new ProcessStartInfo("sqllocaldb.exe")
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        startInfo.ArgumentList.Add(command);
        startInfo.ArgumentList.Add(instance);
        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Không thể chạy sqllocaldb.exe.");
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(15000))
        {
            process.Kill(entireProcessTree: true);
            throw new InvalidOperationException("LocalDB không phản hồi trong 15 giây.");
        }
        if (process.ExitCode != 0)
            throw new InvalidOperationException($"Không thể {command} LocalDB '{instance}': {error.GetAwaiter().GetResult()} {output.GetAwaiter().GetResult()}");
        return output.GetAwaiter().GetResult();
    }
}
