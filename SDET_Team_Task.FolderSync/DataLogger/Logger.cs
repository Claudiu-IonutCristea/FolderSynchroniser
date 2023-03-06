using SDET_Team_Task.FolderSync.CLIArguments;
using SDET_Team_Task.FolderSync.Synchronisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDET_Team_Task.FolderSync.DataLogger;
internal class Logger
{
	private protected string _logFilePath;

	public Logger(string logFilePath)
	{
		_logFilePath = logFilePath;
	}

	public virtual async Task WriteModifiedFilesAsync(IEnumerable<string> modifiedFilePaths)
	{
		foreach(var path in modifiedFilePaths)
		{
			await WriteLineAsync(path);
		}
	}

	public async Task WriteLineAsync(string line)
	{
		Console.WriteLine(line);

		using StreamWriter sw = File.AppendText(_logFilePath);

		await sw.WriteLineAsync(line);
	}

	public async Task WriteLineAsync()
	{
		Console.WriteLine();

		using StreamWriter sw = File.AppendText(_logFilePath);

		await sw.WriteLineAsync();
	}

	public async Task WriteSettingsAsync(Settings settings)
	{
		await WriteLineAsync($"Source folder: {settings.SourceFolderPath}");
		await WriteLineAsync($"Replica folder: {settings.ReplicaFolderPath}");
		await WriteLineAsync($"Log file: {settings.LogFilePath}");
		await WriteLineAsync($"Sync period: {TimeSpan.FromMilliseconds(settings.SyncPeriodMs):dd\\.hh\\:mm\\:ss}");
	}
}
