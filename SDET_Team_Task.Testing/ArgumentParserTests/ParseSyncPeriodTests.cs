using SDET_Team_Task.FolderSync.ErrorHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDET_Team_Task.FolderSync.CLIArguments.ArgumentParser;

namespace SDET_Team_Task.Testing.ArgumentParserTests;
internal class ParseSyncPeriodTests
{
	[TestCaseSource(nameof(ValidInput_Data))]
	public static void ValidInput_Tests(string syncPeriod, TimeSpan expectedTs)
	{
		Assert.That(ParseSyncPeriod(syncPeriod), Is.EqualTo(expectedTs));
	}

	public static object[] ValidInput_Data =
	{
		new object[] { "1h10m5s",	new TimeSpan( 1, 10,  5) },
		new object[] { "10m5s",		new TimeSpan( 0, 10,  5) },
		new object[] { "1h5s",		new TimeSpan( 1,  0,  5) },
		new object[] { "1h10m",		new TimeSpan( 1, 10,  0) },
		new object[] { "1h",		new TimeSpan( 1,  0,  0) },
		new object[] { "10m",		new TimeSpan( 0, 10,  0) },
		new object[] { "5s",		new TimeSpan( 0,  0,  5) },

		new object[] { "1d",		new TimeSpan(1,  0,  0,  0) },
		new object[] { "1d2h",		new TimeSpan(1,  2,  0,  0) },
		new object[] { "1d2m",		new TimeSpan(1,  0,  2,  0) },
		new object[] { "1d10s",		new TimeSpan(1,  0,  0, 10) },
		new object[] { "1d2h5m",	new TimeSpan(1,  2,  5,  0) },
		new object[] { "1d2h10s",	new TimeSpan(1,  2,  0, 10) },
		new object[] { "1d5m10s",	new TimeSpan(1,  0,  5, 10) },
		new object[] { "1d2h5m10s",	new TimeSpan(1,  2,  5, 10) },

		//ALMOST TimeSpan.MaxValue
		new object[] { "10675199d02h48m05s", new TimeSpan(10675199, 2, 48, 5) }
	};

	[TestCaseSource(nameof(InvalidInput_Data))]
	public static void InvalidInput_Tests(string syncPeriod, ErrorCategory expectedError, int expectedErrorCode, string expectedErrorSource)
	{
		var result = ParseSyncPeriod(syncPeriod);
		Assert.Multiple(() =>
		{
			Assert.That(result, Is.EqualTo(new TimeSpan(0, 0, -1)));
			Assert.That(ErrorsManager.HasErrorFromCategory(expectedError));
			Assert.That(ErrorsManager.Errors, Does.Contain(new Error
			{
				ErrorCode = expectedErrorCode,
				ErrorSource = expectedErrorSource
			}));
		});

		ErrorsManager.ClearAll();
	}

	public static object[] InvalidInput_Data =
	{
		new object[] { "900h10m5s", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "99m5s", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "25.2h", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "92h", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "66m", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "67s", ErrorCategory.SyncPeriod, 201, string.Empty },

		new object[] { "2147483648d", ErrorCategory.SyncPeriod, 201, string.Empty }, //int.maxValue + 1
		new object[] { "-2147483649d", ErrorCategory.SyncPeriod, 201, string.Empty }, //int.maxValue - 1
		new object[] { "2147483647d", ErrorCategory.SyncPeriod, 201, string.Empty }, //int.maxValue
		new object[] { "-2147483648d", ErrorCategory.SyncPeriod, 201, string.Empty }, //int.maxValue

		new object[] { "-1d", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "1d-2h", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "1d*2h", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "(1d)2h", ErrorCategory.SyncPeriod, 201, string.Empty },

		new object[] { "asdgr", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "null", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "", ErrorCategory.SyncPeriod, 201, string.Empty },

		//zero padding (apparently it is not allowed)
		new object[] { "001d002h005m0010s", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "1d002h005m0010s", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "1d2h005m0010s", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "1d2h5m0010s", ErrorCategory.SyncPeriod, 201, string.Empty },
		new object[] { "1d2h5m010s", ErrorCategory.SyncPeriod, 201, string.Empty },
	};
}
