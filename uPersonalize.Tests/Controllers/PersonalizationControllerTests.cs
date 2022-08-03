using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using uPersonalize.Controllers;

namespace uPersonalize.Tests.Controllers
{
	[TestClass]
	public class PersonalizationControllerTests : TestClassBase
	{
		private PersonalizationController _personalizationController;

		[TestInitialize]
		public void Setup()
		{
			SetupBase();

			_personalizationController = new PersonalizationController(MoqProvider.PersonalizationService());
		}

		[TestMethod]
		public async Task Is_Opt_Out()
		{
			
		}

		[TestMethod]
		public async Task Trigger_Valid_Event()
		{
			await _personalizationController.TriggerEvent("test");

			var count = await _personalizationController.GetTriggeredEventCount("test");
		}

		[TestMethod]
		public async Task Trigger_Invalid_Event()
		{
			await _personalizationController.TriggerEvent(string.Empty);

			var count = await _personalizationController.GetTriggeredEventCount(string.Empty);

			Assert.AreEqual(0, count);
		}
	}
}