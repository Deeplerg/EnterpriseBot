﻿using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using VkNet.Model.RequestParams;

namespace EnterpriseBot.VK.Models.MenuRelated
{
    public class NextAction : ICloneable
    {
        private NextAction() { }

        public NextAction(Type menu, string methodName = nameof(IMenu.DefaultMenuLayout), params MenuParameter[] pars)
                   : this(menu, TypeUtils.GetMethod(menu, methodName), pars) { }

        public NextAction(Type menu, MethodInfo action, params MenuParameter[] pars)
        {
            MenuAssemblyName = menu.Assembly.GetName().Name;
            MenuTypeName = menu.Name;
            MenuNamespace = menu.Namespace;
            MenuActionMethodName = action?.Name;
            Parameters = pars;
        }

        public string MenuAssemblyName { get; set; }
        public string MenuNamespace { get; set; }
        public string MenuTypeName { get; set; }
        public string MenuActionMethodName { get; set; }

        [JsonIgnore]
        public MenuParameter[] Parameters
        {
            get => parameters.ToArray();
            set
            {
                if (value != null)
                    parameters = value.ToList();
            }
        }
        [JsonProperty]
        private List<MenuParameter> parameters = new List<MenuParameter>();

        public object Clone()
        {
            return new NextAction
            {
                MenuAssemblyName = MenuAssemblyName,
                MenuNamespace = MenuNamespace,
                MenuTypeName = MenuTypeName,
                MenuActionMethodName = MenuActionMethodName,
                Parameters = parameters.ConvertAll(parameter => (MenuParameter)parameter.Clone()).ToArray()
            };
        }
    }
}
