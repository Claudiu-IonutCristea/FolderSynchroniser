using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDET_Team_Task.FolderSync.Synchroniser;

namespace SDET_Team_Task.Testing.Synchroniser_Tests.Parse_Command_Line_Arguments_Tests;
internal class ReplicaFolderPathTests
{
	[TestCase(new string[] { "-r", "C:\\folder\\replica" }, "C:\\folder\\replica")]
	[TestCase(new string[] { "-R", "C:\\folder\\replica" }, "C:\\folder\\replica")]
	[TestCase(new string[] { "-r", "\"C:\\test folder\\replica\"" }, "C:\\test folder\\replica")]
	[TestCase(new string[] { "-R", "\"C:\\test folder\\replica\"" }, "C:\\test folder\\replica")]
	public void AbsolutePathTest(string[] args, string expected)
	{
		var resultString = ParseCommandLineArguments(args).ReplicaFolderPath;

		Assert.That(resultString, Is.EqualTo(expected));
	}

	[TestCase(new string[] { "-r", @"./replica" }, @"\replica")]
	[TestCase(new string[] { "-r", @".\replica" }, @"\replica")]
	[TestCase(new string[] { "-R", @"./replica" }, @"\replica")]
	[TestCase(new string[] { "-R", @".\replica" }, @"\replica")]
	[TestCase(new string[] { "-R", "\"./replica folder\"" }, @"\replica folder")]
	public void RelativePathTest(string[] args, string folderName)
	{
		var fullPathExpected = Directory.GetCurrentDirectory() + folderName;

		var resultString = ParseCommandLineArguments(args).ReplicaFolderPath;

		Assert.That(resultString, Is.EqualTo(fullPathExpected));
	}

	[TestCase(new string[] { "-r", "" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-r", "   " }, typeof(ArgumentException))]
	[TestCase(new string[] { "-r", "\t" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-r", "C:\\test:case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-r", "C:\\test*case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-r", "C:\\test?case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-r", "C:\\test<case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-r", "C:\\test>case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "-r", "C:\\test|case" }, typeof(ArgumentException))]
	[TestCase(new string[] { "r", "C:\\test" }, typeof(ArgumentException))]
	[TestCase(new string[] { "R", "C:\\test" }, typeof(ArgumentException))]
	[TestCase(new string[] { " ", "C:\\test" }, typeof(ArgumentException))]

	[TestCase(new string[] { "-r", "C:\\test", "-s", "C:\\test" }, typeof(Exception))]
	public void InvalidPathTest(string[] args, Type expectedExceptionType)
	{
		Assert.Throws(expectedExceptionType, () => ParseCommandLineArguments(args));
	}
}
