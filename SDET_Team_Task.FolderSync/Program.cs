using SDET_Team_Task.FolderSync.CLIArguments;
using SDET_Team_Task.FolderSync.DataLogger;
using SDET_Team_Task.FolderSync.ErrorHandling;
using SDET_Team_Task.FolderSync.Synchronisation;
using System.Timers;

namespace SDET_Team_Task.FolderSync
{
	internal class Program
	{
		static async Task<int> Main(string[] args)
		{
			var settings = ArgumentParser.Parse(args);
			if(settings == null || ErrorsManager.HasErrors)
			{
				Console.Write(await ErrorsManager.WriteAllErrorsAsync());
				return ErrorsManager.Errors.First().ErrorCode;
			}

			//Doesn't affect the directories if they already exist
			Directory.CreateDirectory(settings.SourceFolderPath);
			Directory.CreateDirectory(settings.ReplicaFolderPath);

			var logger = new Logger(settings.LogFilePath);
			var timer = new System.Timers.Timer(settings.SyncPeriodMs);

			await SynchroniseFoldersAsync(settings, logger);

			timer.Elapsed += async (sender, e) => await SynchroniseFoldersAsync(settings, logger);
			timer.AutoReset = true;
			timer.Enabled = true;

			var consoleLine = "";
			while(consoleLine != "quit" && consoleLine != "q")
			{
				consoleLine = Console.ReadLine();
			}

			timer.Stop();
			timer.Dispose();

			Console.WriteLine("Terminating the application...");

			//returns first error code from the list of errors
			//returns 0 if there are no errors
			return ErrorsManager.HasErrors ? ErrorsManager.Errors.First().ErrorCode : 0;
		}

		private static async Task SynchroniseFoldersAsync(Settings settings, Logger logger)
		{
			var sourceFiles = Synchroniser.GetAllFilesAndDirectories(settings.SourceFolderPath);
			var replicaFiles = Synchroniser.GetAllFilesAndDirectories(settings.ReplicaFolderPath);

			var syncActions = Synchroniser.GetSyncActions(sourceFiles, replicaFiles, settings);

			await Synchroniser.SynchroniseAsync(settings, syncActions, logger);

			Console.WriteLine("Type \"quit\" or \"q\" to terminate the app");
		}
	}
}