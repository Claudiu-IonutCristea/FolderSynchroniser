using SDET_Team_Task.FolderSync.CLIArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDET_Team_Task.FolderSync.ErrorHandling;
using static SDET_Team_Task.FolderSync.CLIArguments.ArgumentParser;

namespace SDET_Team_Task.Testing.ArgumentParserTests;
internal class ParseTests : BaseTestSetup
{
	[TestCaseSource(nameof(ValidArgs_Data))]
	public static void ValidArgs_Tests(string[] args, Settings expectedSettings)
	{
		Assert.Multiple(() =>
		{
			Assert.That(Parse(args), Is.EqualTo(expectedSettings));

			Assert.That(ErrorsManager.HasErrors, Is.EqualTo(false));
		});
	}

	public static object[] ValidArgs_Data =
	{
		new object[]
		{
			new string[]
			{
				"-s", @"C:\source Folder",
				"-r", @"C:\replica",
				"-l", @"C:\log.txt",
				"-i", "1h",
			},
			new Settings
			{
				SourceFolderPath = @"C:\source Folder",
				ReplicaFolderPath = @"C:\replica",
				LogFilePath = @"C:\log.txt",
				SyncPeriodMs = (int)new TimeSpan(1, 0, 0).TotalMilliseconds,
			}
		},
		new object[]
		{
			new string[]
			{
				"-i", "1h",
				"-r", @"C:\replica",
				"-l", @"C:\log.txt",
				"-s", @"C:\source Folder",
			},
			new Settings
			{
				SourceFolderPath = @"C:\source Folder",
				ReplicaFolderPath = @"C:\replica",
				LogFilePath = @"C:\log.txt",
				SyncPeriodMs = (int)new TimeSpan(1, 0, 0).TotalMilliseconds
			}
		}, //command order
		new object[]
		{
			new string[]
			{
				"-l", @"C:\log folder",
			},
			new Settings
			{
				LogFilePath = @"C:\log folder\log.txt",
			}
		}, //log without \ and .txt
		new object[]
		{
			new string[]
			{
				"-l", @"C:\log folder\",
			},
			new Settings
			{
				LogFilePath = @"C:\log folder\log.txt",
			}
		}, //log without .txt
		new object[]
		{
			new string[]
			{
				"-l", @"C:\logFile.txt",
			},
			new Settings
			{
				LogFilePath = @"C:\logFile.txt",
			}
		}, //log with .txt
		new object[]
		{
			new string[]
			{
				"-l", @"C:\logFile.TXT",
			},
			new Settings
			{
				LogFilePath = @"C:\logFile.TXT",
			}
		}, //log with .TXT
		new object[]
		{
			new string[]
			{
				"-r", @"replica",
				"-l", @"./log.txt",
			},
			new Settings
			{
				ReplicaFolderPath = Directory.GetCurrentDirectory() + @"\replica",
				LogFilePath = Directory.GetCurrentDirectory() + @"\log.txt",
			}
		}, //relative paths
		new object[]
		{
			new string[]
			{
				"-source", @"C:\source Folder",
				"-replica", @"C:\replica",
				"-log", @"C:\log.txt",
				"-sync", "1h",
			},
			new Settings
			{
				SourceFolderPath = @"C:\source Folder",
				ReplicaFolderPath = @"C:\replica",
				LogFilePath = @"C:\log.txt",
				SyncPeriodMs = (int)new TimeSpan(1, 0, 0).TotalMilliseconds,
			}
		}, //full name commands
		new object[]
		{
			Array.Empty<string>(),
			new Settings(),
		}, //empty args
	};

	[TestCaseSource(nameof(InvalidArgs_Data))]
	public static void InvalidArgs_Tests(string[] args, Error[] expectedErrors)
	{
		var settings = Parse(args);
		Assert.Multiple(() =>
		{
			Assert.That(settings, Is.Null);
			Assert.That(ErrorsManager.HasErrorFromCategory(ErrorCategory.ArgumentsParsing));
			Assert.That(ErrorsManager.Errors.Intersect(expectedErrors), Is.EquivalentTo(expectedErrors));
			Assert.That(ErrorsManager.Errors.Except(expectedErrors), Is.Empty);
		});
	}

	public static object[] InvalidArgs_Data =
	{
		new object[]
		{
			new string[]
			{
				"source", @"C:\source Folder",
			},
			new Error[]
			{
				new Error { ErrorCode = 301, ErrorSource = "source" },
			},
		},
		new object[]
		{
			new string[]
			{
				"-s", @"C:\folder",
				"-r", @"C:\folder",
			},
			new Error[]
			{
				new Error { ErrorCode = 302, ErrorSource = "" },
			},
		},
		new object[]
		{
			new string[]
			{
				"-asdf", @"C:\folder",
			},
			new Error[]
			{
				new Error { ErrorCode = 303, ErrorSource = "-ASDF" },
			},
		},
		new object[]
		{
			new string[]
			{
				"-l", @"C:\log.pdf",
			},
			new Error[]
			{
				new Error { ErrorCode = 304, ErrorSource = ".pdf" },
			},
		},
	};
}
