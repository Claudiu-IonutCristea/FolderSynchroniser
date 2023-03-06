using SDET_Team_Task.FolderSync.CLIArguments;
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

	public static void Synchronise(Settings settings, Dictionary<string, SyncActionTypes> syncActions)
	{
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
	}

	private static bool IsDirectory(string path)
		=> File.GetAttributes(path).HasFlag(FileAttributes.Directory);
}
