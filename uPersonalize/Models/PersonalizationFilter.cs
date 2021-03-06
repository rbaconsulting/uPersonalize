using Newtonsoft.Json;
using uPersonalize.Enums;

namespace uPersonalize.Models
{
	public class PersonalizationFilter
	{
		public PersonalizationConditions Condition { get; set; }
		public PersonalizationActions Action { get; set; }
		public string IpAddress { get; set; }
		public DeviceTypes DeviceToMatch { get; set; }
		public string PageId { get; set; }
		public string EventName { get; set; }
		public int PageEventCount { get; set; }
		public string AdditionalClasses { get; set; }

		public PersonalizationFilter()
		{
			Condition = PersonalizationConditions.Default;
			Action = PersonalizationActions.Default;
			IpAddress = string.Empty;
			DeviceToMatch = DeviceTypes.Default;
			PageId = string.Empty;
			EventName = string.Empty;
			PageEventCount = 0;
			AdditionalClasses = string.Empty;
		}

		public static PersonalizationFilter Create(string json) => !string.IsNullOrWhiteSpace(json) ? JsonConvert.DeserializeObject<PersonalizationFilter>(json) : new PersonalizationFilter();

		public bool IsValid()
		{
			if(Condition != PersonalizationConditions.Default && Action != PersonalizationActions.Default)
            {
				switch (Condition)
				{
					case PersonalizationConditions.IP_Address: return DeviceToMatch != DeviceTypes.Default;
					case PersonalizationConditions.Device_Type: return DeviceToMatch != DeviceTypes.Default;
					case PersonalizationConditions.Visited_Page: return !string.IsNullOrWhiteSpace(PageId);
					case PersonalizationConditions.Visited_Page_Count: return !string.IsNullOrWhiteSpace(PageId) && PageEventCount > 0;
					case PersonalizationConditions.Event_Triggered: return !string.IsNullOrWhiteSpace(EventName);
					case PersonalizationConditions.Event_Triggered_Count: return !string.IsNullOrWhiteSpace(EventName) && PageEventCount > 0;
					case PersonalizationConditions.Logged_In: return true;
					default: break;
				}
			}		

			return false;
		}
	}
}