using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDET_Team_Task.FolderSync.Synchroniser;

namespace SDET_Team_Task.Testing.Synchroniser_Tests.Parse_Command_Line_Arguments_Tests;
internal class ReplicaFolderPathTests
{
	[TestCase(new string[] { "-s", "C:\\folder\\replica" }, @"C:\folder\replica")]
	[TestCase(new string[] { "-S", "C:\\folder\\replica" }, @"C:\folder\replica")]
	[TestCase(new string[] { "-s", "\"C:\\test folder\\replica\"" }, @"C:\test folder\replica")]
	[TestCase(new string[] { "-S", "\"C:\\test folder\\replica\"" }, @"C:\test folder\replica")]
	public void AbsolutePathTest(string[] args, string expected)
	{
		var resultString = ParseCommandLineArguments(args).SourceFolderPath;

		Assert.That(resultString, Is.EqualTo(expected));
	}

	[TestCase(new string[] { "-s", @"./replica" }, @"\replica")]
	[TestCase(new string[] { "-s", @".\replica" }, @"\replica")]
	[TestCase(new string[] { "-S", @"./replica" }, @"\replica")]
	[TestCase(new string[] { "-S", @".\replica" }, @"\replica")]
	[TestCase(new string[] { "-S", "\"./replica folder\"" }, @"\replica folder")]
	public void RelativePathTest(string[] args, string folderName)
	{
		var fullPathExpected = Directory.GetCurrentDirectory() + folderName;

		var resultString = ParseCommandLineArguments(args).SourceFolderPath;

		Assert.That(resultString, Is.EqualTo(fullPathExpected));
	}

	[TestCase(new string[] { "-s", "" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-s", "   " }, typeof(ArgumentException))]
	[TestCase(new string[] { "-s", "\t" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-s", "C:\\test:case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-s", "C:\\test*case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-s", "C:\\test?case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-s", "C:\\test<case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-s", "C:\\test>case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-s", "C:\\test|case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "s", "C:\\test" }, typeof(ArgumentException))]
	[TestCase(new string[] { "S", "C:\\test" }, typeof(ArgumentException))]
	[TestCase(new string[] { " ", "C:\\test" }, typeof(ArgumentException))]
	public void InvalidPathTest(string[] args, Type expectedExceptionType)
	{
		Assert.Throws(expectedExceptionType, () => ParseCommandLineArguments(args));
	}
}
