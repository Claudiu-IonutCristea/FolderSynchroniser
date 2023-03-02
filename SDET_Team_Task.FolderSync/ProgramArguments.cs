namespace SDET_Team_Task.FolderSync;
internal class ProgramArguments
{
	public string SourceFolderPath { get; set; } = Directory.GetCurrentDirectory() + "\\source";
	public string ReplicaFolderPath { get; set; } = Directory.GetCurrentDirectory() + "\\replica";
	public string LogFilePath { get; set; } = Directory.GetCurrentDirectory() + "\\log.txt";
	public int SyncPeriodMs { get; set; } = 10_000; //10 seconds
	public bool LogTreeView { get; set; } = false;

}
