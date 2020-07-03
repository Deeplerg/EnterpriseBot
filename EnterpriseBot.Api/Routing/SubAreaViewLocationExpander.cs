using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.Api.Routing
{
    public class SubAreaViewLocationExpander : IViewLocationExpander
    {
		private const string subAreaRouteKey = "subarea";

		public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
		{
			if (context.ActionContext.RouteData.Values.ContainsKey(subAreaRouteKey))
			{
				string subAreaValue = RazorViewEngine.GetNormalizedRouteValue(context.ActionContext, subAreaRouteKey);

				IEnumerable<string> subAreaViewLocation = new string[]
				{
					"/Areas/{0}/SubAreas/" + subAreaValue + "/Views/{1}/{2}.cshtml"
				};

				viewLocations = subAreaViewLocation.Concat(viewLocations);
			}

			return viewLocations;
		}

		public void PopulateValues(ViewLocationExpanderContext context)
		{
			string subAreaValue = string.Empty;
			context.ActionContext.ActionDescriptor.RouteValues.TryGetValue(subAreaRouteKey, out subAreaValue);

			context.Values[subAreaRouteKey] = subAreaValue;
		}
	}
}
