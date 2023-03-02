using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDET_Team_Task.FolderSync.Synchroniser;

namespace SDET_Team_Task.Testing.Synchroniser_Tests.Parse_Command_Line_Arguments_Tests;
internal class SyncPeriodTests
{
	[TestCaseSource(nameof(SyncPeriodValidCases))]
	public void SyncPeriodValidTests(string[] args, int expectedMs)
	{
		var result = ParseCommandLineArguments(args).SyncPeriodMs;

		Assert.That(result, Is.EqualTo(expectedMs));
	}

	public static object[] SyncPeriodValidCases =
	{
		new object[] { new string[] { "-i", "1h10m5s" },	(int)new TimeSpan(1, 10, 5).TotalMilliseconds },
		new object[] { new string[] { "-i", "10m5s" },		(int)new TimeSpan(0, 10, 5).TotalMilliseconds },
		new object[] { new string[] { "-i", "1h5s" },		(int)new TimeSpan(1, 00, 5).TotalMilliseconds },
		new object[] { new string[] { "-i", "1h10m" },		(int)new TimeSpan(1, 10, 0).TotalMilliseconds },
		new object[] { new string[] { "-i", "1h" },			(int)new TimeSpan(1, 0, 0).TotalMilliseconds },
		new object[] { new string[] { "-i", "10m" },		(int)new TimeSpan(0, 10, 0).TotalMilliseconds },
		new object[] { new string[] { "-i", "50s" },		(int)new TimeSpan(0, 0, 50).TotalMilliseconds },

		//Capital i
		new object[] { new string[] { "-I", "50s" },		(int)new TimeSpan(0, 0, 50).TotalMilliseconds },

		new object[] { new string[] { "-i", "1d" },			(int)new TimeSpan(1, 0, 0, 0).TotalMilliseconds },
		new object[] { new string[] { "-i", "1d2h" },		(int)new TimeSpan(1, 2, 0, 0).TotalMilliseconds },
		new object[] { new string[] { "-i", "1d2m" },		(int)new TimeSpan(1, 0, 2, 0).TotalMilliseconds },
		new object[] { new string[] { "-i", "1d10s" },		(int)new TimeSpan(1, 0, 0, 10).TotalMilliseconds },
		new object[] { new string[] { "-i", "1d2h5m" },		(int)new TimeSpan(1, 2, 5, 0).TotalMilliseconds },
		new object[] { new string[] { "-i", "1d2h10s" },	(int)new TimeSpan(1, 2, 0, 10).TotalMilliseconds },
		new object[] { new string[] { "-i", "1d2m10s" },	(int)new TimeSpan(1, 0, 2, 10).TotalMilliseconds },
		new object[] { new string[] { "-i", "1d2h5m10s" },	(int)new TimeSpan(1, 2, 5, 10).TotalMilliseconds },

		new object[] { new string[] { "-i", "7d0h0m0s" },	(int)new TimeSpan(7, 0, 0, 0).TotalMilliseconds },

		//*Almost* TimeSpan.MaxValue
		new object[] { new string[] { "-i", "10675199d02h48m05s" }, (int)new TimeSpan(10675199, 2, 48, 5).TotalMilliseconds },

		//Default value if no argument is passed
		new object[] { Array.Empty<string>(),		(int)new TimeSpan(0, 00, 10).TotalMilliseconds },
	};

	[TestCaseSource(nameof(SyncPeriodInvalidCases))]
	public void SyncPeriodInvalidTests(string[] args, Type expectedExceptionType)
	{
		Assert.Throws(expectedExceptionType, () => ParseCommandLineArguments(args));
	}

	public static object[] SyncPeriodInvalidCases =
	{
		new object[] { new string[] { "-i", "900h10m5s" },  typeof(ArgumentException) },
		new object[] { new string[] { "-i", "99m5s" },      typeof(ArgumentException) },
		new object[] { new string[] { "-i", "25.2h5s" },    typeof(ArgumentException) },
		new object[] { new string[] { "-i", "92h" },        typeof(ArgumentException) },
		new object[] { new string[] { "-i", "66m" },        typeof(ArgumentException) },
		new object[] { new string[] { "-i", "67s" },        typeof(ArgumentException) },
								
		new object[] { new string[] { "-io", "50s" },       typeof(ArgumentException) },

		new object[] { new string[] { "-i", "2147483648d" }, typeof(ArgumentException) }, //int.maxValue + 1
		new object[] { new string[] { "-i", "-2147483649d" },typeof(ArgumentException) }, //int.minValue - 1
		new object[] { new string[] { "-i", "2147483647d" }, typeof(ArgumentException) }, //int.maxValue
		new object[] { new string[] { "-i", "-2147483648d" },typeof(ArgumentException) }, //int.minValue
		new object[] { new string[] { "-i", "-1d2h" },       typeof(ArgumentException) },
		new object[] { new string[] { "-i", "1d-2h" },       typeof(ArgumentException) },
		new object[] { new string[] { "-i", "1d*2h" },       typeof(ArgumentException) },
		new object[] { new string[] { "-i", "(1d)*2h" },     typeof(ArgumentException) },

		new object[] { new string[] { "-i", "dsafeaf" },    typeof(ArgumentException) },
		new object[] { new string[] { "-i", "dhms" },		typeof(ArgumentException) },
		new object[] { new string[] { "-i", "null" },		typeof(ArgumentException) },

		new object[] { new string[] { "-i", "001d002h005m0010s" },  typeof(ArgumentException) },
		new object[] { new string[] { "-i", "1d002h005m0010s" },  typeof(ArgumentException) },
		new object[] { new string[] { "-i", "1d2h005m0010s" },  typeof(ArgumentException) },
		new object[] { new string[] { "-i", "1d2h5m0010s" },  typeof(ArgumentException) },

	};
}
