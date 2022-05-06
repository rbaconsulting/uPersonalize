using uPersonalize.Models;
using System.Threading.Tasks;

namespace uPersonalize.Interfaces
{
	public interface IPersonalizationService
	{
		Task TrackUser(int pageId);
		Task<bool> IsMatch(PersonalizationFilter filter);
		Task<bool> TryTriggerEvent(string eventName);
		Task<bool> TryPageVisit(string pageId);
		Task<int> GetTriggeredEventCount(string eventName);
		Task<int> GetPageVisitCount(string pageId);
	}
}