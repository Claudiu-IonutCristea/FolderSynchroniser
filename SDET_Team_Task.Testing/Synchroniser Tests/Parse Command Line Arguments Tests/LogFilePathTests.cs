using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDET_Team_Task.FolderSync.Synchroniser;

namespace SDET_Team_Task.Testing.Synchroniser_Tests.Parse_Command_Line_Arguments_Tests;
internal class LogFilePathTests
{
	[TestCase(new string[] { "-l", "C:\\folder" }, "C:\\folder\\log.txt")]
	[TestCase(new string[] { "-L", "C:\\folder" }, "C:\\folder\\log.txt")]
	[TestCase(new string[] { "-l", "\"C:\\test folder\"" }, "C:\\test folder\\log.txt")]
	[TestCase(new string[] { "-L", "\"C:\\test folder\"" }, "C:\\test folder\\log.txt")]

	[TestCase(new string[] { "-l", "C:\\folder\\" }, "C:\\folder\\log.txt")]
	[TestCase(new string[] { "-l", "C:\\folder\\.txt" }, "C:\\folder\\.txt")]
	[TestCase(new string[] { "-l", "C:\\folder\\synclog.txt" }, "C:\\folder\\synclog.txt")]
	public void AbsolutePathTest(string[] args, string expected)
	{
		var resultString = ParseCommandLineArguments(args).LogFilePath;

		Assert.That(resultString, Is.EqualTo(expected));
	}

	[TestCase(new string[] { "-l", @"./folder" }, @"\folder\log.txt")]
	[TestCase(new string[] { "-l", @".\folder" }, @"\folder\log.txt")]
	[TestCase(new string[] { "-L", @"./folder" }, @"\folder\log.txt")]
	[TestCase(new string[] { "-L", @".\folder" }, @"\folder\log.txt")]
	[TestCase(new string[] { "-l", @".\folder\synclog.txt" }, @"\folder\synclog.txt")]
	[TestCase(new string[] { "-l", @".\folder\" }, @"\folder\log.txt")]
	[TestCase(new string[] { "-l", @".\folder\.txt" }, @"\folder\.txt")]
	[TestCase(new string[] { "-L", "\"./folder folder\"" }, @"\folder folder\log.txt")]
	public void RelativePathTest(string[] args, string folderName)
	{
		var fullPathExpected = Directory.GetCurrentDirectory() + folderName;

		var resultString = ParseCommandLineArguments(args).LogFilePath;

		Assert.That(resultString, Is.EqualTo(fullPathExpected));
	}

	[TestCase(new string[] { "-l", "" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-l", "   " }, typeof(ArgumentException))]
	[TestCase(new string[] { "-l", "\t" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-l", "C:\\test:case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-l", "C:\\test*case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-l", "C:\\test?case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-l", "C:\\test<case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-l", "C:\\test>case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-l", "C:\\test|case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "l", "C:\\test" }, typeof(ArgumentException))]
	[TestCase(new string[] { "L", "C:\\test" }, typeof(ArgumentException))]
	[TestCase(new string[] { " ", "C:\\test" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-l", "C:\\test\\log.pdf" }, typeof(ArgumentException))]
	public void InvalidPathTest(string[] args, Type expectedExceptionType)
	{
		Assert.Throws(expectedExceptionType, () => ParseCommandLineArguments(args));
	}
}
