using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SDET_Team_Task.FolderSync.ErrorHandling;

namespace SDET_Team_Task.FolderSync.CLIArguments;
internal static class ArgumentParser
{
	/// <summary>
	/// Parses CLI Arguments and returns new <see cref="Settings"/> object
	/// </summary>
	/// <remarks>
	///	If <see cref="string"/>[] <paramref name="args"/> is empty, returns <see cref="Settings"/> with default values
	/// </remarks>
	/// <param name="args"></param>
	/// <returns>
	/// new <see cref="Settings"/> object
	/// <br/><br/>
	/// If any of the arguments are not valid, returns <see langword="null"/> and adds the <see cref="Error"/> to the <see cref="ErrorsManager"/><br/>
	/// <see cref="ErrorCategory.ArgumentsParsing"/><br/>
	/// <see cref="ErrorCategory.PathValidation"/><br/>
	/// <see cref="ErrorCategory.SyncPeriod"/>
	/// </returns>
	public static Settings? Parse(string[] args)
    {
        var programArgs = new Settings();

        //No arguments passed
        //Use default values
        if (args.Length == 0)
        {
            return programArgs;
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (!args[i].StartsWith('-'))
            {
                ErrorsManager.Add(301, args[i]);
                return null;
            }

            args[i] = args[i].ToUpper();

            switch (args[i])
            {
                #region Source Folder
                case "-S":
				case "-SOURCE":
					i++;
                    var sourceFolder = ParsePath(args[i]);
                    if (ErrorsManager.HasErrorFromCategory(ErrorCategory.PathValidation))
                        break;

                    programArgs.SourceFolderPath = sourceFolder;

                    if (programArgs.ReplicaFolderPath == programArgs.SourceFolderPath)
                    {
                        ErrorsManager.Add(302);
                        break;
                    }

                    break;
                #endregion

                #region Replica Folder
                case "-R":
				case "-REPLICA":
					i++;
                    var replicaFolder = ParsePath(args[i]);
                    if (ErrorsManager.HasErrorFromCategory(ErrorCategory.PathValidation))
                        break;

                    programArgs.ReplicaFolderPath = replicaFolder;

                    if (programArgs.ReplicaFolderPath == programArgs.SourceFolderPath)
                    {
                        ErrorsManager.Add(302);
                        break;
                    }

                    break;
                #endregion

                #region Sync Period
                case "-I":
				case "-SYNC":
					i++;
                    var syncTs = ParseSyncPeriod(args[i]);
                    if (ErrorsManager.HasErrorFromCategory(ErrorCategory.SyncPeriod))
                        break;

                    programArgs.SyncPeriodMs = (int)syncTs.TotalMilliseconds;

                    break;
                #endregion

                #region Log File Path
                case "-L":
				case "-LOG":
					i++;
                    var path = ParsePath(args[i]);
                    if (ErrorsManager.HasErrorFromCategory(ErrorCategory.PathValidation))
                        break;

                    var extension = Path.GetExtension(path).ToLower();

                    if (!string.IsNullOrEmpty(extension))
                    {
                        if (extension == ".txt")
                        {
                            programArgs.LogFilePath = path;
                            break;
                        }

                        ErrorsManager.Add(304, extension);
                        break;
                    }

                    //if there is no extension that means path ends with directory name
                    programArgs.LogFilePath = path + (Path.EndsInDirectorySeparator(path) ? "log.txt" : "\\log.txt");
                    break;
                #endregion

                default:
                    ErrorsManager.Add(303, args[i]);
                    return null;
            }
        }

		if(ErrorsManager.HasErrors)
			return null;

        return programArgs;
    }

	/// <summary>
	/// Parses a given path into a valid ABSOLUTE path
	/// </summary>
	/// <param name="path">Path to parse</param>
	/// <returns>
	/// An absolute path <seealso cref="string"/><br/>
	/// <br/><br/>
	/// If path is not valid returns an empty path and adds the <see cref="Error"/> to the <see cref="ErrorsManager"/><br/>
	/// <see cref="ErrorCategory.PathValidation"/>
	/// </returns>
    public static string ParsePath(string path)
    {
        var result = Path.GetFullPath(path);

        if (!IsPathValid(result))
        {
            return string.Empty;
        }

        return result;
    }

	/// <summary>
	/// Converts a <see cref="string"/> with a special format into a <see cref="TimeSpan"/> meant to be used for the syncPeriod
	/// </summary>
	/// <remarks>
	/// See documentation for string format and examples.<br/>
	/// <see href="https://github.com/Claudiu-IonutCristea/SDET_Team_Task#sync-interval--i-timeinterval-or--sync-timeinterval"/>
	/// </remarks>
	/// <param name="syncPeriod">formatted <see cref="string"/></param>
	/// <returns>
	/// <see cref="TimeSpan"/> based on the <paramref name="syncPeriod"/><br/>
	/// <br/><br/>
	/// If path is not valid returns a new <see cref="TimeSpan"/>(0, 0, -1) and adds the <see cref="Error"/> to the <see cref="ErrorsManager"/><br/>
	/// <see cref="ErrorCategory.SyncPeriod"/>
	/// </returns>
	public static TimeSpan ParseSyncPeriod(string syncPeriod)
    {
        var formats = new string[] { @"s\s", @"m\m", @"h\h", @"h\hm\m", @"h\hs\s", @"m\ms\s", @"h\hm\ms\s",
            @"d\d", @"d\dh\h", @"d\dm\m", @"d\ds\s", @"d\dh\hm\m", @"d\dh\hs\s", @"d\dm\ms\s", @"d\dh\hm\ms\s"
        };

        if (!TimeSpan.TryParseExact(syncPeriod, formats, null, out TimeSpan ts))
        {
            ErrorsManager.Add(201);
            return new TimeSpan(0, 0, -1);
        }

        return ts;
    }

    private static bool IsPathValid(string path)
    {
        //Check if the path is rooted in a drive
		//Redundant check in this case
		//	Path.GetFullPath() will always make a path that is at least 3 chars long or will throw Exception
        if (path.Length < 3)
        {
            ErrorsManager.Add(101, path);
            return false;
        }
        var driveCheck = new Regex(@"^[a-zA-Z]:\\$");
        if (!driveCheck.IsMatch(path[..3]))
        {
            ErrorsManager.Add(102, path[..3]);
            return false;
        }

        //Check if drive exists
        IEnumerable<string> allMachineDrivers = DriveInfo.GetDrives().Select(drive => drive.Name);
        if (!allMachineDrivers.Contains(path[..3]))
        {
            ErrorsManager.Add(103, path[..3]);
            return false;
        }

        //Check if the rest of the path is valid
        var invalidFileNameChars = new string(Path.GetInvalidPathChars());
        invalidFileNameChars += @":/?*><" + "\"";
        var containsABadCharacter = new Regex("[" + Regex.Escape(invalidFileNameChars) + "]");
        if (containsABadCharacter.IsMatch(path[3..]))
        {
            ErrorsManager.Add(104, path);
            return false;
        }

        return true;
    }
}
