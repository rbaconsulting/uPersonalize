using Lucene.Net.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web;
using Umbraco.Cms.Web.Common;
using uPersonalize.Enums;
using uPersonalize.Models;

namespace uPersonalize.Extensions
{
	public static class uPersonalizeExtensions
	{
		public static PersonalizationFilter MapJPropertyToFilter(JProperty property, UmbracoHelper umbracoHelper)
		{
			var json = property.Value.ToString();

			return JsonConvert.DeserializeObject<PersonalizationFilter>(json);
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
			else if (isMatch && filter.Action == PersonalizationActions.Additional_Classes && !string.IsNullOrWhiteSpace(filter.AdditionalClasses))
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