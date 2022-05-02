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


        public PersonalizationFilter()
        {      
            Condition = PersonalizationConditions.Default;
            Action = PersonalizationActions.Default;
            IpAddress = string.Empty;
            DeviceToMatch = DeviceTypes.Default;
            PageId = string.Empty;
            EventName = string.Empty;
            PageEventCount = 0;
        }
    }
}