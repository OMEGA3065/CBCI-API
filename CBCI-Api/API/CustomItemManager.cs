using System.Reflection;
using HarmonyLib;
using LabApi.Features.Console;

namespace CustomItemLib.API
{
    /// <summary>
    /// Manages namespaces and registration of items.
    /// </summary>
    public static class CustomItemManager
    {
        /// <summary>
        /// The list of all registered items.
        /// </summary>
        /// <returns>The list of all registered items according to their namespace.</returns>
        public static readonly Dictionary<ItemNamespace, ICustomItem<object>> items = new();

        /// <summary>
        /// Registers a Custom Item Definition.
        /// </summary>
        /// <param name="item">The custom item definition to register.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Whether or not the item was registered successfully.</returns>
        public static bool RegisterItem<T>(ICustomItem<T> item)
        {
            if (items.ContainsKey(item.Namespace))
                return false;
            var typed = item as ICustomItem<object>;
            items[item.Namespace] = typed;
            return true;
        }

        /// <summary>
        /// Unregisters a Custom Item Definition.
        /// </summary>
        /// <param name="item">The custom item definition to unregister.</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Whether or not the item was unregistered successfully.</returns>
        public static bool UnregisterItem(ICustomItem<object> item)
        {
            return items.Remove(item.Namespace);
        }

        /// <summary>
        /// Used for obtaining an item based on a namespace.
        /// </summary>
        /// <param name="itemNamespace">The namespace of the target item.</param>
        /// <param name="item">The resulting item.</param>
        /// <returns>Whether or not the namespace has an item definition assigned to it.</returns>
        public static bool TryGetItem(ItemNamespace itemNamespace, out ICustomItem<object> item)
        {
            return items.TryGetValue(itemNamespace, out item);
        }

        /// <summary>
        /// Used for obtaining an item based on a namespace. Autocasts the result to a target <see cref="CustomItemLib.API.ItemInstanceBase">.
        /// </summary>
        /// <param name="itemNamespace">The namespace of the target item.</param>
        /// <param name="item">The resulting item.</param>
        /// <typeparam name="T">What <see cref="CustomItemLib.API.ItemInstanceBase"> to cast to.</typeparam>
        /// <returns>Whether or not the namespace has an item definition assigned to it.</returns>
        public static bool TryGetItem<T>(ItemNamespace itemNamespace, out ICustomItem<T> item)
        {
            bool success = items.TryGetValue(itemNamespace, out var itemUncast);
            if (!success) { item = null; return false; }
            if (itemUncast is ICustomItem<T> castItem)
            {
                item = castItem;
                return true;
            }
            item = null;
            return false;
        }

        /// <summary>
        /// Registers all Item definitions in a specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to search for item definitions.</param>
        /// <returns>An <see cref="IEnumerable{ICustomItem{object}}"/> of definitions that have been successfully registered.</returns>
        public static IEnumerable<ICustomItem<object>> RegisterAllItems(Assembly assembly)
        {
            IEnumerable<ICustomItem<object>> registeredItems = [];
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface || !typeof(ICustomItem<object>).IsAssignableFrom(type))
                    continue;
                try
                {
                    var item = (ICustomItem<object>)Activator.CreateInstance(type);
                    if (RegisterItem(item))
                    {
                        registeredItems = registeredItems.AddItem(item);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to create instance of {type}! Error: {ex}");
                }
            }
            return registeredItems;
        }

        /// <summary>
        /// Registers all Item definitions in a <see cref="Assembly.GetCallingAssembly"/> assembly.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{ICustomItem{object}}"/> of definitions that have been successfully registered.</returns>
        public static IEnumerable<ICustomItem<object>> RegisterAllItems()
        {
            return RegisterAllItems(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Unregisters all Item definitions from a specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to search for item definitions.</param>
        /// <returns>An <see cref="IEnumerable{ICustomItem{object}}"/> of definitions that have been successfully registered.</returns>
        public static IEnumerable<ICustomItem<object>> UnregisterAllItems(Assembly assembly)
        {
            IEnumerable<ICustomItem<object>> unregisteredItems = [];
            foreach (var item in items.Values)
            {
                if (item.GetType().Assembly != assembly) continue;
                if (UnregisterItem(item))
                    unregisteredItems = unregisteredItems.AddItem(item);
            }
            return unregisteredItems;
        }

        /// <summary>
        /// Unregisters all Item definitions in a <see cref="Assembly.GetCallingAssembly"/> assembly.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{ICustomItem{object}}"/> of definitions that have been successfully registered.</returns>
        public static IEnumerable<ICustomItem<object>> UnregisterAllItems()
        {
            return UnregisterAllItems(Assembly.GetCallingAssembly());
        }
    }
}