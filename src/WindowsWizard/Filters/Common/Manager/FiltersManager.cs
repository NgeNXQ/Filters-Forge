using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using WindowsWizard.Filters.Common.Base;

namespace WindowsWizard.Filters.Common.Manager
{
    internal sealed class FiltersManager
    {
        private static readonly FiltersManager instance;

        private readonly SortedDictionary<string, Filter> filters;

        internal static FiltersManager Instance { get => instance; }

        internal ICollection<string> FiltersAliases { get => filters.Keys; }

        static FiltersManager()
        {
            instance = new FiltersManager();
        }

        private FiltersManager()
        {
            this.filters = new SortedDictionary<string, Filter>();

            IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes().Where(child => child.IsSubclassOf(typeof(Filter)));

            foreach (Type type in types)
            {
                Filter? instance = Activator.CreateInstance(type) as Filter;

                if (instance != null)
                {
                    this.filters.Add(instance.Alias, instance);
                }
            }
        }

        internal Filter? this[string alias]
        {
            get
            {
                return this.filters.TryGetValue(alias, out Filter? filter) ? filter : null;
            }
        }
    }
}
