using uPersonalize.Constants;
using uPersonalize.Enums.Attributes;

namespace uPersonalize.Enums
{
	public enum PersonalizationConditions
	{
		Default = 0,
		IP_Address = 1,
		[CookieName(Cookies.Device.Name)]
		Device_Type = 2,
		[CookieName(Cookies.VisitedPages.Name)]
		Visited_Page = 3,
		[CookieName(Cookies.VisitedPages.Name)]
		Visited_Page_Count = 4,
		[CookieName(Cookies.ClickedEvents.Name)]
		Event_Triggered = 5,
		[CookieName(Cookies.ClickedEvents.Name)]
		Event_Triggered_Count = 6,
		Logged_In = 7,
		DateTime_Before = 8,
		DateTime_After = 9
	}
}