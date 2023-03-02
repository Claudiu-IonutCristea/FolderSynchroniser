using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDET_Team_Task.FolderSync.Synchroniser;

namespace SDET_Team_Task.Testing.Synchroniser_Tests.Parse_Command_Line_Arguments_Tests;
internal class LogTreeViewTests
{
	[TestCase(new string[] {"-t", "true"}, true)]
	[TestCase(new string[] {"-T", "true"}, true)]
	[TestCase(new string[] {"-t", "TRUE"}, true)]
	[TestCase(new string[] {"-t", "TrUe"}, true)]
	[TestCase(new string[] {"-t", "false"}, false)]
	[TestCase(new string[] {"-T", "false"}, false)]
	[TestCase(new string[] {"-t", "FALSE"}, false)]
	[TestCase(new string[] {"-t", "FaLsE"}, false)]
	[TestCase(new string[] { }, false)]
	public void ArgumentValid(string[] args, bool expected)
	{
		var result = ParseCommandLineArguments(args).LogTreeView;

		Assert.That(result, Is.EqualTo(expected));
	}

	[TestCase(new string[] {"-t", "ture"}, typeof(ArgumentException))]
	[TestCase(new string[] {"-t", "asdf"}, typeof(ArgumentException))]
	[TestCase(new string[] {"-T", "asdf"}, typeof(ArgumentException))]
	[TestCase(new string[] {"-T", ""}, typeof(ArgumentException))]
	public void ArgumentInvalid(string[] args, Type expectedExceptionType)
	{
		Assert.Throws(expectedExceptionType, () => ParseCommandLineArguments(args));
	}
}
