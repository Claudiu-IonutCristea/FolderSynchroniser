using SDET_Team_Task.FolderSync.CLIArguments;
using SDET_Team_Task.FolderSync.ErrorHandling;
using SDET_Team_Task.FolderSync.Synchronisation;

namespace SDET_Team_Task.FolderSync
{
	internal class Program
	{
		static int Main(string[] args)
		{
			var timer = new System.Timers.Timer(2000);

			timer.Elapsed += Timer_Elapsed;
			timer.AutoReset = true;
			timer.Enabled = true;

			Console.WriteLine("\nPress the Enter key to exit the application...\n");
			Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
			Console.ReadLine();
			timer.Stop();
			timer.Dispose();

			Console.WriteLine("Terminating the application...");

			//returns first error code from the list of errors
			//returns 0 if there are no errors
			return ErrorsManager.HasErrors ? ErrorsManager.Errors.First().ErrorCode : 0;
		}

		private static void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
		{
			Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}", e.SignalTime);
		}
	}
}