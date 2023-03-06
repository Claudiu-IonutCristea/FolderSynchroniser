using SDET_Team_Task.FolderSync.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDET_Team_Task.Testing.ErrorHandlingTests;
internal class ErrorManagerTests : BaseTestSetup
{
	[TestCaseSource(nameof(WriteAllErrorsAsync_Data))]
	public static async Task WriteAllErrorsAsync_Tests(Error[] errors, string expectedString)
	{
		foreach(var error in errors)
		{
			ErrorsManager.Add(error);
		}

		Assert.That(await ErrorsManager.WriteAllErrorsAsync(), Is.EqualTo(expectedString));
	}

	public static object[] WriteAllErrorsAsync_Data =
	{
		new object[]
		{   
			//Error[] errors
			new Error[] {
				new Error() { ErrorCode = 102, ErrorSource = @"3:\" },
				new Error() { ErrorCode = 201 },
				new Error() { ErrorCode = 303, ErrorSource = "-x" },
				new Error() { ErrorCode = 999 },
			},

			//string expectedString
			"Drive letter 3:\\ is invalid!" + Environment.NewLine +
			"Sync period invalid format!" + Environment.NewLine +
			"Command -x not recognised!" + Environment.NewLine +
			"Unknown error" + Environment.NewLine
		}
	};
}
