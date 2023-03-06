using SDET_Team_Task.FolderSync.CLIArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;
using System.Security.Principal;

namespace SDET_Team_Task.Testing;
internal abstract class BaseDirectoriesTestSetup : BaseTestSetup
{
	public const string TEST_SOURCE_PATH = @"./Testing Source";
	public const string TEST_REPLICA_PATH = @"./Testing Replica";

	public static readonly string fullPathSource = Path.GetFullPath(TEST_SOURCE_PATH);
	public static readonly string fullPathReplica = Path.GetFullPath(TEST_REPLICA_PATH);

	public static readonly Settings settings = new()
	{
		SourceFolderPath = fullPathSource,
		ReplicaFolderPath = fullPathReplica,
	};

	public static readonly string[] sourceFiles = new string[] //Based on [SetUp]
	{
		Path.Combine(fullPathSource, @"dir a"),
		Path.Combine(fullPathSource, @"dir a\a.txt"),
		Path.Combine(fullPathSource, @"dir a\dir aa"),
		Path.Combine(fullPathSource, @"dir a\dir aa\a.txt"),
		Path.Combine(fullPathSource, @"dir a\dir aa\b.txt"),
		Path.Combine(fullPathSource, @"dir a\dir ab"),
		Path.Combine(fullPathSource, @"dir a\dir ab\a.txt"),
		Path.Combine(fullPathSource, @"dir b"),
		Path.Combine(fullPathSource, @"dir b\a.txt"),
		Path.Combine(fullPathSource, @"dir b\dir ba"),
		Path.Combine(fullPathSource, @"dir b\dir ba\a.txt"),
		Path.Combine(fullPathSource, @"dir b\dir ba\b.txt"),
		Path.Combine(fullPathSource, @"dir b\dir ba\inBothDifferentWriteDate.txt"),
		Path.Combine(fullPathSource, @"dir b\dir ba\inBothSameWriteDate.txt"),
		Path.Combine(fullPathSource, @"dir b\dir bb"),
		Path.Combine(fullPathSource, @"dir b\dir bb\dir bba"),
		Path.Combine(fullPathSource, @"dir b\dir bb\dir bba\a.txt"),
		Path.Combine(fullPathSource, @"dir b\dir bc"),

	};

	public static readonly string[] replicaFiles = new string[] //Based on [SetUp]
	{
		Path.Combine(fullPathReplica, @"dir b"),
		Path.Combine(fullPathReplica, @"dir b\dir ba"),
		Path.Combine(fullPathReplica, @"dir b\dir ba\inBothDifferentWriteDate.txt"),
		Path.Combine(fullPathReplica, @"dir b\dir ba\inBothSameWriteDate.txt"),
		Path.Combine(fullPathReplica, @"dir b\only in replica.txt"),
		Path.Combine(fullPathReplica, @"will be deleted"),
	};

	private static readonly DirectoryInfo _sourceDir = new(TEST_SOURCE_PATH);
	private static readonly DirectoryInfo _replicaDir = new(TEST_REPLICA_PATH);

	public override void SetUp()
	{
		base.SetUp();

		if(_sourceDir.Exists)
			_sourceDir.Delete(true);

		if(_replicaDir.Exists)
			_replicaDir.Delete(true);


		//SOURCE DIR FILES
		_sourceDir.Create();
		_sourceDir.CreateSubdirectory(@"dir a");
		File.WriteAllText(Path.Combine(TEST_SOURCE_PATH, @"dir a\a.txt"), "will be copied to replica");
		_sourceDir.CreateSubdirectory(@"dir a\dir aa");
		File.WriteAllText(Path.Combine(TEST_SOURCE_PATH, @"dir a\dir aa\a.txt"), "will be copied to replica");
		File.WriteAllText(Path.Combine(TEST_SOURCE_PATH, @"dir a\dir aa\b.txt"), "will be copied to replica");
		_sourceDir.CreateSubdirectory(@"dir a\dir ab");
		File.WriteAllText(Path.Combine(TEST_SOURCE_PATH, @"dir a\dir ab\a.txt"), "will be copied to replica");
		_sourceDir.CreateSubdirectory(@"dir b");
		File.WriteAllText(Path.Combine(TEST_SOURCE_PATH, @"dir b\a.txt"), "will be copied to replica");
		_sourceDir.CreateSubdirectory(@"dir b\dir ba");
		File.WriteAllText(Path.Combine(TEST_SOURCE_PATH, @"dir b\dir ba\a.txt"), "will be copied to replica");
		File.WriteAllText(Path.Combine(TEST_SOURCE_PATH, @"dir b\dir ba\b.txt"), "will be copied to replica");
		_sourceDir.CreateSubdirectory(@"dir b\dir bb");
		_sourceDir.CreateSubdirectory(@"dir b\dir bb\dir bba");
		File.WriteAllText(Path.Combine(TEST_SOURCE_PATH, @"dir b\dir bb\dir bba\a.txt"), "will be copied to replica");
		_sourceDir.CreateSubdirectory(@"dir b\dir bc");

		File.WriteAllText(Path.Combine(TEST_SOURCE_PATH, @"dir b\dir ba\inBothSameWriteDate.txt"), "will be left alone");
		File.SetCreationTime(Path.Combine(TEST_SOURCE_PATH, @"dir b\dir ba\inBothSameWriteDate.txt"), new DateTime(2023, 3, 6));
		File.SetLastWriteTime(Path.Combine(TEST_SOURCE_PATH, @"dir b\dir ba\inBothSameWriteDate.txt"), new DateTime(2023, 3, 6));

		File.WriteAllText(Path.Combine(TEST_SOURCE_PATH, @"dir b\dir ba\inBothDifferentWriteDate.txt"), "will replace other file");
		File.SetCreationTime(Path.Combine(TEST_SOURCE_PATH, @"dir b\dir ba\inBothDifferentWriteDate.txt"), new DateTime(2023, 3, 6));
		File.SetLastWriteTime(Path.Combine(TEST_SOURCE_PATH, @"dir b\dir ba\inBothDifferentWriteDate.txt"), new DateTime(2023, 3, 6));
		

		//REPLICA DIR FILES
		_replicaDir.Create();
		_replicaDir.CreateSubdirectory(@"dir b");
		_replicaDir.CreateSubdirectory(@"dir b\dir ba");
		_replicaDir.CreateSubdirectory(@"will be deleted"); //folder doesn't exist in source

		File.WriteAllText(Path.Combine(TEST_REPLICA_PATH, @"dir b\only in replica.txt"), "will be deleted");

		File.WriteAllText(Path.Combine(TEST_REPLICA_PATH, @"dir b\dir ba\inBothDifferentWriteDate.txt"), "will be replaced");
		File.SetCreationTime(Path.Combine(TEST_REPLICA_PATH, @"dir b\dir ba\inBothDifferentWriteDate.txt"), new DateTime(2023, 3, 2));
		File.SetLastWriteTime(Path.Combine(TEST_REPLICA_PATH, @"dir b\dir ba\inBothDifferentWriteDate.txt"), new DateTime(2023, 3, 2));

		File.WriteAllText(Path.Combine(TEST_REPLICA_PATH, @"dir b\dir ba\inBothSameWriteDate.txt"), "will be left alone");
		File.SetCreationTime(Path.Combine(TEST_REPLICA_PATH, @"dir b\dir ba\inBothSameWriteDate.txt"), new DateTime(2023, 3, 6));
		File.SetLastWriteTime(Path.Combine(TEST_REPLICA_PATH, @"dir b\dir ba\inBothSameWriteDate.txt"), new DateTime(2023, 3, 6));
	}

	public override void TearDown()
	{
		base.TearDown();
		_sourceDir.Delete(true);
		_replicaDir.Delete(true);
	}
}
