using System;
using System.Reflection;

namespace EnterpriseBot.VK.Utils
{
    public static class TypeUtils
    {
        public static MethodInfo GetMethod(Type type, string name)
        {
            return type.GetMethod(name,
                                  BindingFlags.Public | BindingFlags.Instance
                                | BindingFlags.Static | BindingFlags.NonPublic);
        }
    }
}
