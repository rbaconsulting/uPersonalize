using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using uPersonalize.Interfaces;
using Moq;
using uPersonalize.Enums;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Security;

namespace uPersonalize.Tests
{
	public static class MoqProvider
	{
		public static ILogger<T> Logger<T>()
		{
			var logger = new Mock<ILogger<T>>();

			return logger.Object;
		}

		public static IPersonalizationSettings PersonalizationSettings()
		{
			var personalizationSettings = new Mock<IPersonalizationSettings>();
			personalizationSettings.Setup(p => p.GetCookieOptions()).Returns(new CookieOptions());

			return personalizationSettings.Object;
		}

		public static IHttpContextAccessor HttpContextAccessor(HttpContext context)
		{
			var httpContextAccessor = new Mock<IHttpContextAccessor>();
			httpContextAccessor.SetupGet(h => h.HttpContext).Returns(context);

			return httpContextAccessor.Object;
		}

		public static IMemberManager MemberManager()
		{
			var httpContextAccessor = new Mock<IMemberManager>();
			httpContextAccessor.SetupGet(h => h.IsLoggedIn()).Returns(false);

			return httpContextAccessor.Object;
		}

		public static IPersonalizationCookieManager PersonalizationCookieManager(bool withValues, bool asList)
		{
			var cookieManager = new Mock<IPersonalizationCookieManager>();
			cookieManager.SetReturnsDefault(Task.FromResult(true));
			cookieManager.SetReturnsDefault(Task.FromResult(string.Empty));

			if(asList)
			{
				cookieManager.Setup(c => c.GetCookie(PersonalizationConditions.Visited_Page)).Returns(Task.FromResult("10:1,11:1"));
				cookieManager.Setup(c => c.GetCookie(PersonalizationConditions.Visited_Page_Count)).Returns(Task.FromResult("10:1,11:1"));
				cookieManager.Setup(c => c.GetCookie(PersonalizationConditions.Event_Triggered)).Returns(Task.FromResult("testEvent:1,testEvent1:1"));
				cookieManager.Setup(c => c.GetCookie(PersonalizationConditions.Event_Triggered_Count)).Returns(Task.FromResult("testEvent:1,testEvent1:1"));
			}
			else if (withValues)
			{
				cookieManager.Setup(c => c.GetCookie(PersonalizationConditions.Device_Type)).Returns(Task.FromResult("Desktop_Windows"));
				cookieManager.Setup(c => c.GetCookie(PersonalizationConditions.Visited_Page)).Returns(Task.FromResult("10:1"));
				cookieManager.Setup(c => c.GetCookie(PersonalizationConditions.Visited_Page_Count)).Returns(Task.FromResult("10:1"));
				cookieManager.Setup(c => c.GetCookie(PersonalizationConditions.Event_Triggered)).Returns(Task.FromResult("testEvent:1"));
				cookieManager.Setup(c => c.GetCookie(PersonalizationConditions.Event_Triggered_Count)).Returns(Task.FromResult("testEvent:1"));
			}			

			return cookieManager.Object;
		}
	}
}