using uPersonalize.Enums.Attributes;

namespace uPersonalize.Enums
{
	public enum PersonalizationConditions
	{
		Default = 0,
		IP_Address = 1,
		[CookieName("uPersonalize_device")]
		Device_Type = 2,
		[CookieName("uPersonalize_visited_pages")]
		Visited_Page = 3,
		[CookieName("uPersonalize_visited_pages")]
		Visited_Page_Count = 4,
		[CookieName("uPersonalize_click_event")]
		Event_Triggered = 5,
		[CookieName("uPersonalize_click_event")]
		Event_Triggered_Count = 6,
		Logged_In = 7,
		DateTime_Before = 8,
		DateTime_After = 9
	}
}