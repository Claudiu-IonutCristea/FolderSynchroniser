using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDET_Team_Task.FolderSync.CLIArguments;
using SDET_Team_Task.FolderSync.DataLogger;
using SDET_Team_Task.FolderSync.Synchronisation;
using static SDET_Team_Task.FolderSync.Synchronisation.Synchroniser;

namespace SDET_Team_Task.Testing.SyncroniserTests;
internal class GetDirectories : BaseDirectoriesTestSetup
{
	[Test]
	public static void GetAllFilesAndDirectories_Test()
	{
		var source = GetAllFilesAndDirectories(fullPathSource);
		Assert.That(source, Is.EquivalentTo(sourceFiles));

		var replica = GetAllFilesAndDirectories(fullPathReplica);
		Assert.That(replica, Is.EquivalentTo(replicaFiles));
	}

	[Test]
	public static void GetSyncActions_Test()
	{
		var result = GetSyncActions(sourceFiles, replicaFiles, settings);

		Assert.Multiple(() => 
		{
			Assert.Multiple(() =>
			{
				Assert.That(result, Does.ContainKey(@"dir b\only in replica.txt"));
				Assert.That(result[@"dir b\only in replica.txt"], Is.EqualTo(SyncActionTypes.Delete));
			});

			Assert.Multiple(() =>
			{
				Assert.That(result, Does.ContainKey(@"dir b\dir ba\inBothDifferentWriteDate.txt"));
				Assert.That(result[@"dir b\dir ba\inBothDifferentWriteDate.txt"], Is.EqualTo(SyncActionTypes.Replace));
			});

			Assert.Multiple(() =>
			{
				Assert.That(result, Does.ContainKey(@"dir a\a.txt"));
				Assert.That(result[@"dir a\a.txt"], Is.EqualTo(SyncActionTypes.Copy));
			});

			Assert.That(result, Does.Not.ContainKey(@"dir b\dir ba\inBothSameWriteDate.txt"));
		});
	}

	[Test]
	public static async Task Synchronise_Test()
	{
		var dictionary = GetSyncActions(sourceFiles, replicaFiles, settings);
		var logger = new Logger(settings.LogFilePath);

		await SynchroniseAsync(settings, dictionary, logger);

		var newReplicaFiles = GetAllFilesAndDirectories(fullPathReplica);

		var sourceRelativePaths = new List<string>();
		var replicaRelativePaths = new List<string>();

		foreach(var path in sourceFiles)
		{
			sourceRelativePaths.Add(Path.GetRelativePath(fullPathSource, path));
		}

		foreach(var path in newReplicaFiles)
		{
			replicaRelativePaths.Add(Path.GetRelativePath(fullPathReplica, path));
		}

		Assert.Multiple(() =>
		{
			Assert.That(replicaRelativePaths, Is.EquivalentTo(sourceRelativePaths));
			Assert.That(newReplicaFiles, Does.Not.Contain(Path.Combine(fullPathReplica, @"dir b\only in replica.txt")));
		});
	}
}
