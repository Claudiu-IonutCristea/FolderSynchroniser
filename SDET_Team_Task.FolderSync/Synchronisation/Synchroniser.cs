using SDET_Team_Task.FolderSync.CLIArguments;
using SDET_Team_Task.FolderSync.DataLogger;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace SDET_Team_Task.FolderSync.Synchronisation;
internal static class Synchroniser
{
	public static string[] GetAllFilesAndDirectories(string path)
	{
		var dirs = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
		var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

		return dirs.Union(files).OrderBy(s => s).ToArray();
	}

	public static Dictionary<string, SyncActionTypes> GetSyncActions(string[] sourceFiles, string[] replicaFiles, Settings settings)
	{
		var result = new Dictionary<string, SyncActionTypes>();
		var source = sourceFiles.Select(s => Path.GetRelativePath(settings.SourceFolderPath, s));
		var replica = replicaFiles.Select(s => Path.GetRelativePath(settings.ReplicaFolderPath, s));

		var filesOnlyInReplica = source.Union(replica).Except(source);
		var filesInBoth = source.Intersect(replica);
		var filesOnlyInSource = source.Except(replica);

		//Files to delete
		foreach(var file in filesOnlyInReplica)
		{
			result.Add(file, SyncActionTypes.Delete);
		}

		//files to replace
		foreach(var file in filesInBoth)
		{
			if(File.GetLastWriteTime(Path.Combine(settings.SourceFolderPath, file)) >
				File.GetLastWriteTime(Path.Combine(settings.ReplicaFolderPath, file)))
			{
				result.Add(file, SyncActionTypes.Replace);
			}
		}

		//files to copy
		foreach(var file in filesOnlyInSource)
		{
			result.Add(file, SyncActionTypes.Copy);
		}

		return result;
	}

	public static async Task SynchroniseAsync(Settings settings, Dictionary<string, SyncActionTypes> syncActions, Logger logger)
	{
		await logger.WriteLineAsync($"Started synchronisation at [{DateTime.Now:yyyy/MM/dd-HH:mm:ss}]");
		await logger.WriteSettingsAsync(settings);
		await logger.WriteLineAsync();

		var sw = new Stopwatch();
		sw.Start();

		foreach(var pair in syncActions)
		{
			var relativePath = pair.Key;
			var syncAction = pair.Value;

			var sourceFullPath = Path.Combine(settings.SourceFolderPath, relativePath);
			var replicaFullPath = Path.Combine(settings.ReplicaFolderPath, relativePath);

			switch(syncAction)
			{
				case SyncActionTypes.Delete:
					if(IsDirectory(replicaFullPath))
						Directory.Delete(replicaFullPath);
					else
						File.Delete(replicaFullPath);
					break;

				case SyncActionTypes.Copy:
					if(IsDirectory(sourceFullPath))
						Directory.CreateDirectory(replicaFullPath);
					else
						File.Copy(sourceFullPath, replicaFullPath);
					break;

				case SyncActionTypes.Replace:
					if(IsDirectory(sourceFullPath))
						break;
					else
						File.Copy(sourceFullPath, replicaFullPath, true);
					break;

				default:
					throw new NotImplementedException("SyncActionType not supported!");
			}

		}

		sw.Stop();

		var deletedFiles = syncActions.Where(x => x.Value == SyncActionTypes.Delete).Select(x => x.Key);
		var replacedFiles = syncActions.Where(x => x.Value == SyncActionTypes.Replace).Select(x => x.Key);
		var newFiles = syncActions.Where(x => x.Value == SyncActionTypes.Copy).Select(x => x.Key);

		await logger.WriteLineAsync($"----- DELETED FILES ({deletedFiles.Count()}) -----");
		await logger.WriteModifiedFilesAsync(deletedFiles);
		await logger.WriteLineAsync();

		await logger.WriteLineAsync($"----- REPLACED FILES ({replacedFiles.Count()}) -----");
		await logger.WriteModifiedFilesAsync(replacedFiles);
		await logger.WriteLineAsync();

		await logger.WriteLineAsync($"----- NEW FILES ({newFiles.Count()}) -----");
		await logger.WriteModifiedFilesAsync(newFiles);
		await logger.WriteLineAsync();

		await logger.WriteLineAsync();
		await logger.WriteLineAsync($"Synchronisation finished in {sw.ElapsedMilliseconds} ms");
		await logger.WriteLineAsync($"Next synchronisation at [{DateTime.Now + TimeSpan.FromMilliseconds(settings.SyncPeriodMs):yyyy/MM/dd-HH:mm:ss}]");
	}

	private static bool IsDirectory(string path)
		=> File.GetAttributes(path).HasFlag(FileAttributes.Directory);
}
