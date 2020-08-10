using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseBot.VK.Extensions
{
    public static class ListExtensions
    {
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            return new ReadOnlyCollection<T>(source.ToList());
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IList<T> source)
        {
            return new ReadOnlyCollection<T>(source);
        }
    }
}
