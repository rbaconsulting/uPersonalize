using uPersonalize.Enums;
using uPersonalize.Models;
using System.Threading.Tasks;

namespace uPersonalize.Interfaces
{
	public interface IPersonalizationService
	{
		Task TrackUser(int pageId);
		Task<bool> IsMatch(PersonalizationFilter filter);
		void TriggerEvent(string eventName);
		void PageVisited(string pageId);
		int GetTriggeredEventCount(string eventName);
		int GetPageVisitCount(string pageId);
	}
}