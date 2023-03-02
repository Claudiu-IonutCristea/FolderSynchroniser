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

		for(int i = 0; i < args.Length / 2; i += 2)
		{
			if(args[i][0] != '-')
			{
				throw new ArgumentException("Invalid command line argument format");
			}

			args[i] = args[i].ToUpper();

			switch(args[i])
			{
				case "-S":
					programArgs.SourceFolderPath = ParseDirectoryPath(args[i + 1]).ToString();
					break;
				case "-R":
					programArgs.ReplicaFolderPath = ParseDirectoryPath(args[i + 1]).ToString();
					break;
				case "-I":
					programArgs.SyncPeriod = ParseSyncPeriod(args[i + 1]);
					break;
				case "-L":
					break;
				case "-T":
					break;

				default:
					break;
			}
		}

		return programArgs;
	}

	private static string ParseDirectoryPath(string path)
	{
		var result = Path.GetFullPath(path.Trim('\"'));

		if(!IsPathValid(result))
		{
			throw new ArgumentException();
		}

		return result;
	}

	private static bool IsPathValid(string path)
	{
		// Check if the path is rooted in a driver
		if(path.Length < 3) return false;
		Regex driveCheck = new Regex(@"^[a-zA-Z]:\\$");
		if(!driveCheck.IsMatch(path[..3])) return false;

		// Check if such driver exists
		IEnumerable<string> allMachineDrivers = DriveInfo.GetDrives().Select(drive => drive.Name);
		if(!allMachineDrivers.Contains(path.Substring(0, 3))) return false;

		// Check if the rest of the path is valid
		string InvalidFileNameChars = new string(Path.GetInvalidPathChars());
		InvalidFileNameChars += @":/?*><" + "\"";
		Regex containsABadCharacter = new Regex("[" + Regex.Escape(InvalidFileNameChars) + "]");
		if(containsABadCharacter.IsMatch(path.Substring(3, path.Length - 3)))
			return false;
		if(path[path.Length - 1] == '.') return false;

		return true;
	}

	private static int ParseSyncPeriod(string syncPeriod)
	{
		var formats = new string[] { @"s\s", @"m\m", @"h\h", @"h\h m\m", @"h\h s\s", @"m\m s\s", @"h\h m\m s\s" };
		if(!TimeSpan.TryParseExact(syncPeriod, formats, null, out TimeSpan ts))
		{
			throw new ArgumentException("sync period invalid format");
		}

		return ts.Milliseconds;
	}
}
