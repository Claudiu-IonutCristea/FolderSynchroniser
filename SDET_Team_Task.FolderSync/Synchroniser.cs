using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace SDET_Team_Task.FolderSync;
internal static class Synchroniser
{
	public static ProgramArguments ParseCommandLineArguments(string[] args)
	{
		var programArgs = new ProgramArguments();

		//No arguments passed
		//Use default values
		if(args.Length == 0)
			return programArgs;

		for(int i = 0; i < args.Length; i += 2)
		{
			if(!args[i].StartsWith('-'))
			{
				throw new ArgumentException("Invalid command line argument format. Commands have to start with \"-\"");
			}

			args[i] = args[i].ToUpper();

			switch(args[i])
			{
				case "-S":
					programArgs.SourceFolderPath = ParsePath(args[i + 1].Trim('\"'));

					if(programArgs.ReplicaFolderPath == programArgs.SourceFolderPath)
						throw new Exception("Source folder and replica folder cannot be the same");

					break;

				case "-R":
					programArgs.ReplicaFolderPath = ParsePath(args[i + 1].Trim('\"'));

					if(programArgs.ReplicaFolderPath == programArgs.SourceFolderPath)
						throw new Exception("Source folder and replica folder cannot be the same");

					break;

				case "-I":
					programArgs.SyncPeriodMs = ParseSyncPeriod(args[i + 1]);
					break;

				case "-L":
					var path = ParsePath(args[i + 1].Trim('\"'));

					var extension = Path.GetExtension(path);

					if(!string.IsNullOrEmpty(extension))
					{
						if(extension == ".txt")
						{
							programArgs.LogFilePath = path;
							break;
						}

						throw new ArgumentException($"Extension {extension} is not supported for the log file. use \".txt\" instead");
					}

					//if there is no extension that means path ends with directory name
					programArgs.LogFilePath = path + (path.EndsWith('\\') ? "log.txt" : "\\log.txt");
					break;

				case "-T":
					args[i + 1] = args[i + 1].ToUpper();

					if(args[i + 1] != "TRUE" && args[i + 1] != "FALSE")
						throw new ArgumentException("Invalid argument for Log tree view, only accepts \"true\" of \"false\" (case insensitive)\n" +
							"default value is FALSE so it is recommended to not use -T argument unless the intention is to set TRUE");

					programArgs.LogTreeView = args[i + 1] == "TRUE";
					break;

				default:
					throw new ArgumentException("Invalid command");
			}
		}

		return programArgs;
	}

	private static string ParsePath(string path)
	{
		var result = Path.GetFullPath(path);

		if(!IsPathValid(result))
		{
			throw new ArgumentException("Path is not valid");
		}

		return result;
	}

	private static bool IsPathValid(string path)
	{
		// Check if the path is rooted in a driver
		if(path.Length < 3)
			return false;
		var driveCheck = new Regex(@"^[a-zA-Z]:\\$");
		if(!driveCheck.IsMatch(path[..3]))
			return false;

		// Check if such driver exists
		IEnumerable<string> allMachineDrivers = DriveInfo.GetDrives().Select(drive => drive.Name);
		if(!allMachineDrivers.Contains(path[..3]))
			return false;

		// Check if the rest of the path is valid
		var InvalidFileNameChars = new string(Path.GetInvalidPathChars());
		InvalidFileNameChars += @":/?*><" + "\"";
		var containsABadCharacter = new Regex("[" + Regex.Escape(InvalidFileNameChars) + "]");
		if(containsABadCharacter.IsMatch(path[3..]))
			return false;

		return true;
	}

	private static int ParseSyncPeriod(string syncPeriod)
	{
		var formats = new string[] { @"s\s", @"m\m", @"h\h", @"h\hm\m", @"h\hs\s", @"m\ms\s", @"h\hm\ms\s",
			@"d\d", @"d\dh\h", @"d\dm\m", @"d\ds\s", @"d\dh\hm\m", @"d\dh\hs\s", @"d\dm\ms\s", @"d\dh\hm\ms\s"
		};
		if(!TimeSpan.TryParseExact(syncPeriod, formats, null, out TimeSpan ts))
		{
			throw new ArgumentException("sync period invalid format");
		}

		return (int)ts.TotalMilliseconds;
	}
}
