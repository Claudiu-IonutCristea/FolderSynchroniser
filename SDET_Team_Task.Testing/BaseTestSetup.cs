using SDET_Team_Task.FolderSync.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDET_Team_Task.Testing;
internal abstract class BaseTestSetup
{
	[SetUp]
	public virtual void SetUp()
	{

	}

	[TearDown]
	public virtual void TearDown()
	{
		ErrorsManager.ClearAll();
	}

}
