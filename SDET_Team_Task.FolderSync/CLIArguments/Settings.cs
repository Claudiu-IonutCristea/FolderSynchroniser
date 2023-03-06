namespace SDET_Team_Task.FolderSync.CLIArguments;
internal class Settings : IEquatable<Settings>
{
    public string SourceFolderPath { get; set; } = Directory.GetCurrentDirectory() + "\\source";
    public string ReplicaFolderPath { get; set; } = Directory.GetCurrentDirectory() + "\\replica";
    public string LogFilePath { get; set; } = Directory.GetCurrentDirectory() + "\\log.txt";
    public int SyncPeriodMs { get; set; } = 10_000; //10 seconds

	public bool Equals(Settings? other)
		=> other != null &&
		SourceFolderPath == other.SourceFolderPath &&
		ReplicaFolderPath == other.ReplicaFolderPath &&
		LogFilePath == other.LogFilePath &&
		SyncPeriodMs == other.SyncPeriodMs;

	public override bool Equals(object? obj)
		=> ReferenceEquals(obj, this) || Equals(obj as Settings);

	public override int GetHashCode()
		=> HashCode.Combine(SourceFolderPath, ReplicaFolderPath, LogFilePath, SyncPeriodMs);
}
