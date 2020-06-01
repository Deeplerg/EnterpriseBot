using EnterpriseBot.VK.Abstractions;
using EnterpriseBot.VK.Attributes;
using EnterpriseBot.VK.Exceptions;
using EnterpriseBot.VK.Models.MenuRelated;
using EnterpriseBot.VK.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EnterpriseBot.VK.Services
{
    public class DefaultMenuRouter : IMenuRouter
    {
        private readonly ILogger<DefaultMenuRouter> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly List<IMenu> menus = new List<IMenu>();

        public DefaultMenuRouter(ILogger<DefaultMenuRouter> logger, IServiceProvider serviceProvider, bool searchInAllAssemblies = false)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;

            menus.AddRange(CreateMenuInstances(MapMenus(searchInAllAssemblies), this.serviceProvider));
        }

        public IMenu GetMenu(Type menuType)
        {
            var menu = menus.SingleOrDefault(menu => menu.GetType() == menuType);

            if (menu == null)
                throw new MenuNotFoundException($"Can't find menu of type {menuType}");
            else
                return menu;
        }

        public IMenu CreateMenuOfType<T>(T menuType, params object[] parameters) where T : Type
        {
            return CreateMenuInstance(menuType, this.serviceProvider, parameters);
        }

        public async Task<IMenuResult> InvokeMenuActionAsync(MethodInfo action, IMenu menuInstance, params MenuParameter[] invocationParams)
        {
            if (menuInstance is null)
                throw new ArgumentNullException($"{nameof(menuInstance)} was null");

            if (menuInstance.GetType() != action.DeclaringType)
            {
                throw new ArgumentException(string.Format(ExceptionTemplates.MethodDoesNotExistInTypeTemplate,
                                                          action.Name, menuInstance.GetType()));
            }

            bool isAsync;
            if (action.ReturnType == typeof(Task<IMenuResult>))
                isAsync = true;
            else if (action.ReturnType == typeof(IMenuResult))
                isAsync = false;
            else
                throw new ArgumentException($"{nameof(action)} return type is neither {nameof(Task<IMenuResult>)} or {nameof(IMenuResult)}");

            object[] parameters;

            if (action.GetParameters().Any())
            {
                bool InvocationParamsContainAllRequiredParams(MenuParameter[] invocationPars, ParameterInfo[] requiredPars)
                {
                    List<MenuParameter> invocationParamsContainAllRequiredParamsCheck_List = new List<MenuParameter>();

                    if (invocationParams != null && invocationParams.Any())
                        invocationParamsContainAllRequiredParamsCheck_List.AddRange(invocationParams);

                    return requiredPars.All(parameter =>
                    {
                        if (parameter.HasDefaultValue)
                            return true;

                        var parameterInInvPars = invocationParamsContainAllRequiredParamsCheck_List
                                                    .FirstOrDefault(menuParameter => menuParameter.Type == parameter.ParameterType
                                                                    || menuParameter.Name == parameter.Name);

                        if (parameterInInvPars != null)
                        {
                            invocationParamsContainAllRequiredParamsCheck_List
                                .Remove(parameterInInvPars);
                            return true;
                        }
                        else return false;
                    });
                }
                IEnumerable<MenuParameter> SortMenuParameters(MenuParameter[] menuParameters, ParameterInfo[] requiredPars)
                {
                    List<MenuParameter> menuParametersCopyForSorting = new List<MenuParameter>();

                    if (menuParameters != null && menuParameters.Any())
                        menuParametersCopyForSorting.AddRange(menuParameters);

                    List<MenuParameter> sortedParameters = new List<MenuParameter>();

                    foreach (var parameter in requiredPars)
                    {
                        //first, search by name
                        //if nothing found, search by type
                        var param = menuParametersCopyForSorting.FirstOrDefault(p => p.Name == parameter.Name)
                                    ?? menuParametersCopyForSorting.FirstOrDefault(p => p.Type == parameter.ParameterType)
                                    ?? menuParametersCopyForSorting.FirstOrDefault(p => parameter.ParameterType.IsAssignableFrom(p.Type))
                                    ?? (parameter.HasDefaultValue ? new MenuParameter(parameter.DefaultValue) : null);

                        if (param == null)
                            throw new ArgumentException($"Unable to find suitable menu parameter for {parameter}");

                        menuParametersCopyForSorting.Remove(param);

                        sortedParameters.Add(param);
                    }

                    return sortedParameters;
                }


                var requiredParams = action.GetParameters();

                if (!InvocationParamsContainAllRequiredParams(invocationParams, requiredParams))
                {
                    throw new InvalidOperationException($"Invocation parameters for action {action.Name} " +
                                                        $"in menu {menuInstance.GetType()} do not contain all " +
                                                        $"required parameters");
                }

                parameters = SortMenuParameters(invocationParams, requiredParams).Select(p => p.Value).ToArray();
            }
            else
            {
                parameters = null;
            }

            if (isAsync)
            {
                return await (Task<IMenuResult>)action.Invoke(menuInstance, parameters);
            }
            else
            {
                return (IMenuResult)action.Invoke(menuInstance, parameters);
            }
        }


        private IEnumerable<Type> MapMenus(bool searchInAllAssemblies)
        {
            Assembly[] assemblies;

            if (searchInAllAssemblies)
            {
                assemblies = AppDomain.CurrentDomain.GetAssemblies();
            }
            else
            {
                assemblies = new Assembly[] { Assembly.GetExecutingAssembly() };
            }

            List<Type> menus = new List<Type>();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (IsMenu(type) && type.IsPublic)
                    {
                        menus.Add(type);
                    }
                }
            }

            return menus.Distinct();
        }

        private IEnumerable<IMenu> CreateMenuInstances(IEnumerable<Type> menuTypes, IServiceProvider serviceProvider)
        {
            return menuTypes.Select(type => CreateMenuInstance(type, serviceProvider))
                            .ToList();
        }

        private IMenu CreateMenuInstance(Type menuType, IServiceProvider serviceProvider, params object[] parameters)
        {
            if (!IsMenu(menuType))
            {
                throw new ArgumentException($"{nameof(menuType)} is not a menu");
            }

            var menuInstance = (IMenu)ActivatorUtilities.CreateInstance(serviceProvider, menuType);

            var properties = menuType.BaseType.GetProperties();
            if (menuType.BaseType.IsAssignableFrom(typeof(MenuBase)) && properties.Any())
            {
                foreach (var parameter in parameters)
                {
                    foreach (var property in properties)
                    {
                        if (property.GetCustomAttribute<MenuBasePropertyAttribute>() != null)
                        {
                            if (property.PropertyType == parameter.GetType())
                            {
                                property.SetValue(menuInstance, parameter);
                            }
                            else
                            {
                                var service = serviceProvider.GetService(property.PropertyType);
                                if (service != null)
                                {
                                    property.SetValue(menuInstance, service);
                                }
                            }
                        }
                    }
                }
            }

            return menuInstance;
        }

        private bool IsMenu(Type type)
        {
            return type.IsClass && !type.IsAbstract && !type.IsInterface
                && type.GetInterface(nameof(IMenu)) != null;
        }
    }
}
