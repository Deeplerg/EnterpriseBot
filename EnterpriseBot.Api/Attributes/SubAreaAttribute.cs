using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace EnterpriseBot.Api.Attributes
{
    // https://github.com/nemi-chand/SubArea.ASPNetCoreMVC/blob/master/src/SubAreaAttribute.cs

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SubAreaAttribute : RouteValueAttribute
    {
        public SubAreaAttribute(string subAreaName)
            : base("subarea", subAreaName)
        {
            if (string.IsNullOrEmpty(subAreaName))
            {
                throw new ArgumentException("Sub area name cannot be null or empty", nameof(subAreaName));
            }
        }
    }
}
