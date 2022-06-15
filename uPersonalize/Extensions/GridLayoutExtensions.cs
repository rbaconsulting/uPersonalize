using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web;
using Umbraco.Cms.Web.Common;
using uPersonalize.Enums;
using uPersonalize.Models;

namespace uPersonalize.Extensions
{
	public static class GridLayoutExtensions
	{
		public static void MapJPropertyToFilter(this PersonalizationFilter filter, JProperty property, UmbracoHelper umbracoHelper)
		{
			switch(property.Name)
			{
				case "uPersonalize-classes":
					filter.AdditionalClasses = property.Value.ToString();
					return;
				case "uPersonalize-conditions":
					filter.Condition = (PersonalizationConditions)Enum.Parse(typeof(PersonalizationConditions), property.Value.ToString());
					return;
				case "uPersonalize-actions":
					filter.Action = (PersonalizationActions)Enum.Parse(typeof(PersonalizationActions), property.Value.ToString());
					return;
				case "uPersonalize-ip":
					filter.IpAddress = property.Value.ToString();
					break;
				case "uPersonalize-device":
					filter.DeviceToMatch = (DeviceTypes)Enum.Parse(typeof(DeviceTypes), property.Value.ToString());
					return;
				case "uPersonalize-page-visit":
					var udi = property.Value.ToString().Replace("umb://document/", "");
					var content = umbracoHelper.Content(udi);

					filter.PageId = content.Id.ToString();
					return;
				case "uPersonalize-event":
					filter.EventName = property.Value.ToString();
					return;
				case "uPersonalize-page-event-count":
					var parseResult = int.TryParse(property.Value.ToString(), out int pageEventCount);
					if (parseResult)
					{
						filter.PageEventCount = pageEventCount;
					}
					return;
				default:
					return;
			}
		}

		public static void ApplyFilterToAttributes(this List<string> attrs, PersonalizationFilter filter, bool isMatch)
		{
			var personalizedValue = string.Empty;

			if ((isMatch && filter.Action == PersonalizationActions.Show) || (!isMatch && filter.Action == PersonalizationActions.Hide))
			{
				personalizedValue = "uPersonalize-show";
			}
			else if ((!isMatch && filter.Action == PersonalizationActions.Show) || (isMatch && filter.Action == PersonalizationActions.Hide))
			{
				personalizedValue = "uPersonalize-hide";
			}
			else if (isMatch && filter.Action == PersonalizationActions.Addition_Styles && !string.IsNullOrWhiteSpace(filter.AdditionalClasses))
			{
				personalizedValue = filter.AdditionalClasses;
			}

			if (!string.IsNullOrWhiteSpace(personalizedValue))
			{
				personalizedValue = HttpUtility.HtmlAttributeEncode(personalizedValue);

				var existingClasses = attrs.Find(a => a.StartsWith("class="));

				if (!string.IsNullOrWhiteSpace(existingClasses))
				{
					var i = attrs.IndexOf(existingClasses);
					attrs[i] = $"{existingClasses.TrimEnd('\"')} {personalizedValue}\"";
				}
				else
				{
					attrs.Add($"class=\"{personalizedValue}\"");
				}
			}
		}
	}
}