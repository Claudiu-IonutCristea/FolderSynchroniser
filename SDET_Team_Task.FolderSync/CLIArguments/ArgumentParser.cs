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
    public static Settings? Parse(string[] args)
    {
        var programArgs = new Settings();

        //No arguments passed
        //Use default values
        if (args.Length == 0)
        {
            return programArgs;
        }

        for (int i = 0; i < args.Length; i += 2)
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
                    var sourceFolder = ParsePath(args[i + 1]);
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
                    var replicaFolder = ParsePath(args[i + 1]);
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
                    var syncTs = ParseSyncPeriod(args[i + 1]);
                    if (ErrorsManager.HasErrorFromCategory(ErrorCategory.SyncPeriod))
                        break;

                    programArgs.SyncPeriodMs = (int)syncTs.TotalMilliseconds;

                    break;
                #endregion

                #region Log File Path
                case "-L":
                    var path = ParsePath(args[i + 1]);
                    if (ErrorsManager.HasErrorFromCategory(ErrorCategory.PathValidation))
                        break;

                    var extension = Path.GetExtension(path);

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
                    programArgs.LogFilePath = path + (path.EndsWith('\\') ? "log.txt" : "\\log.txt");
                    break;
                #endregion

                #region Log Tree View
                case "-T":
                    programArgs.LogTreeView = true;
                    break;
                #endregion

                default:
                    ErrorsManager.Add(303, args[i]);
                    break;
            }
        }

        return programArgs;
    }

    public static string ParsePath(string path)
    {
        var result = Path.GetFullPath(path);

        if (!IsPathValid(result))
        {
            return string.Empty;
        }

        return result;
    }
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
        // Check if the path is rooted in a drive
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

        // Check if drive exists
        IEnumerable<string> allMachineDrivers = DriveInfo.GetDrives().Select(drive => drive.Name);
        if (!allMachineDrivers.Contains(path[..3]))
        {
            ErrorsManager.Add(103, path[..3]);
            return false;
        }

        // Check if the rest of the path is valid
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
