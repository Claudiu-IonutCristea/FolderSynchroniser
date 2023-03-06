using SDET_Team_Task.FolderSync.CLIArguments;
using SDET_Team_Task.FolderSync.DataLogger;
using SDET_Team_Task.FolderSync.Synchronisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDET_Team_Task.Testing.DataLoggerTests;
internal class LoggerTests : BaseTestSetup
{
	private const string LOG_FILE_PATH = @"./log.txt";

	private static Logger _logger = new(LOG_FILE_PATH);

	public override void SetUp()
	{
		base.SetUp();
		_logger = new(LOG_FILE_PATH);
	}

	public override void TearDown()
	{
		base.TearDown();
		File.Delete(LOG_FILE_PATH);
	}

	[TestCaseSource(nameof(WriteModifiedFilesAsync_Data))]
	public static async Task WriteModifiedFilesAsync_Tests(IEnumerable<string> modifiedFilePaths)
	{
		await _logger.WriteModifiedFilesAsync(modifiedFilePaths);
		
	}

	public static object[] WriteModifiedFilesAsync_Data =
	{
		new object[]
		{
			new List<string> 
			{ 
				@"folder a\file a.txt",
				@"folder a\folder ab",
				@"folder a\folder ab\file a.txt",
				@"folder a\folder ab\file b.pdf",
			},
		},
	};
}
