using SDET_Team_Task.FolderSync.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDET_Team_Task.FolderSync.CLIArguments.ArgumentParser;

namespace SDET_Team_Task.Testing.ArgumentParserTests;
internal class ParsePathTests
{
	[TestCase(@"C:\folder\replica", @"C:\folder\replica")]
	[TestCase(@"C:\folder\replica folder", @"C:\folder\replica folder")]
	public void AbsolutePath_ValidPath_Tests(string path, string expectedFullPath)
	{
		Assert.That(ParsePath(path), Is.EqualTo(expectedFullPath));
	}

	[TestCase(@"./replica", "replica")]
	[TestCase(@".\replica", "replica")]
	[TestCase(@"./replica folder", "replica folder")]
	[TestCase(@"replica folder", "replica folder")]
	public void RelativePath_ValidPath_Tests(string path, string expectedDirectoryName)
	{
		var expectedFullPath = Directory.GetCurrentDirectory() + '\\' + expectedDirectoryName;

		Assert.That(ParsePath(path), Is.EqualTo(expectedFullPath));
	}

	//101 Length invalid
	//102 Drive letter invalid
	[TestCase(@"3:\test", ErrorCategory.PathValidation, 102, @"3:\")]
	//103 Drive does not exist
	[TestCase(@"A:\test", ErrorCategory.PathValidation, 103, @"A:\")]
	//104 Invalid character
	[TestCase(@"C:\test : case", ErrorCategory.PathValidation, 104, @"C:\test : case")]
	[TestCase(@"C:\test * case", ErrorCategory.PathValidation, 104, @"C:\test * case")]
	[TestCase(@"C:\test ? case", ErrorCategory.PathValidation, 104, @"C:\test ? case")]
	[TestCase(@"C:\test < case", ErrorCategory.PathValidation, 104, @"C:\test < case")]
	[TestCase(@"C:\test > case", ErrorCategory.PathValidation, 104, @"C:\test > case")]
	[TestCase(@"C:\test | case", ErrorCategory.PathValidation, 104, @"C:\test | case")]
	public void InvalidPath_Tests(string path, ErrorCategory expectedError, int expectedErrorCode, string expectedErrorSource)
	{
		var result = ParsePath(path);
		Assert.Multiple(() =>
		{
			Assert.That(result, Is.EqualTo(string.Empty));
			Assert.That(ErrorsManager.HasErrorFromCategory(expectedError));
			Assert.That(ErrorsManager.Errors, Does.Contain(new Error
			{
				ErrorCode = expectedErrorCode,
				ErrorSource = expectedErrorSource
			}));
		});

		ErrorsManager.ClearAll();
	}
}
