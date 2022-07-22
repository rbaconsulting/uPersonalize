using uPersonalize.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using uPersonalize.Services;

namespace uPersonalize.Interfaces
{
	/// <summary>
	/// Service already implemented by <see cref="PersonalizationService">PersonalizationService</see>, providing personalization logic to Umbraco.
	/// </summary>
	public interface IPersonalizationService
	{
		/// <summary>
		/// Returns if the current user has opted out or not.
		/// </summary>
		/// <returns>true on if a user has opted out, false if not.</returns>
		Task<bool> IsOptOut();

		/// <summary>
		/// Opts out the current user from being tracked by uPersonalize.
		/// </summary>
		/// <returns>true on success, false on failure.</returns>
		Task<bool> OptOut();

		/// <summary>
		/// Resets all personalization data for the current user, excludes opt out cookie by default.
		/// </summary>
		/// <param name="includeOptOut"></param>
		/// <returns>true on success, false on failure.</returns>
		Task<bool> ResetPersonalization(bool includeOptOut = false);

		/// <summary>
		/// Records a user's page loads, device type, and ip address to be used later by the personalization filter.
		/// </summary>
		/// <param name="pageId"></param>
		/// <returns>true on success, false on failure.</returns>
		Task<bool> OnPageLoad(int pageId);

		/// <summary>
		/// Checks to see if a filter matches based off of personlization conditions and actions.
		/// Should not be used if <see cref="ApplyFilterToGrid">ApplyFilterToGrid</see> method is already being used.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns>true if the filter matches and actions should be applied, false if no actions should be applied.</returns>
		Task<bool> DoesFilterMatch(PersonalizationFilter filter);

		/// <summary>
		/// Checks to see if a filter matches and then applies personalization actions to the attributes used within Umbraco's grid layout property.
		/// </summary>
		/// <param name="attrs"></param>
		/// <param name="filter"></param>
		/// <returns>List of strings to replace the Umbraco's grid attributes</returns>
		Task<List<string>> ApplyFilterToGrid(List<string> attrs, PersonalizationFilter filter);

		/// <summary>
		/// Triggers an event defined by a string name to be used later by the personalization filter.
		/// </summary>
		/// <param name="eventName"></param>
		/// <returns>true on success, false on failure.</returns>
		Task<bool> TriggerEvent(string eventName);

		/// <summary>
		/// Records that a user has loaded/visited a page. If <see cref="OnPageLoad">OnPageLoad</see> is already used within the solution, do not use this method or page loads will be recored more than once!
		/// </summary>
		/// <param name="pageId"></param>
		/// <returns>true on success, false on failure.</returns>
		Task<bool> RecordPageLoad(string pageId);

		/// <summary>
		/// Returns the number of times an event has been triggered.
		/// </summary>
		/// <param name="eventName"></param>
		Task<int> GetTriggeredEventCount(string eventName);

		/// <summary>
		/// Returns the number of times a page has been loaded.
		/// </summary>
		/// <param name="pageId"></param>
		Task<int> GetPageLoadCount(string pageId);
	}
}