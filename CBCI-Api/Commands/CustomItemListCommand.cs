using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using CustomItemLib.API;
using LabApi.Features.Wrappers;
using Utils;

namespace CustomItemLib.Commands
{
    [CommandHandler(typeof(CustomItemCommand))]
    public class CustomItemListCommand : ICommand, IUsageProvider
    {
        private const string PrintAllItemsFlag = "--all";
        public string Command { get; } = "list";
        public string[] Aliases { get; } = new string[] { "l" };
        public string Description { get; } = "Lists the avalible custom items";
        public string[] Usage { get; } = new string[] { "[PluginNamespace]" };

        private bool TryGetRAPlayerIds(ArraySegment<string> arguments, int startIndex, out List<ReferenceHub> hubs)
        {
            hubs = RAUtils.ProcessPlayerIdOrNamesList(arguments, startIndex, out _);
            return hubs != null;
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 1)
            {
                string targetPluginSpace = arguments.First();
                if (targetPluginSpace == PrintAllItemsFlag)
                {
                    response = $"{CustomItemManager.items.Count} items were found in all namespaces\nList of items: [\n{CustomItemManager.items.Select(kvp => $"{kvp.Key} => {kvp.Value.Name}").Aggregate((a, b) => $"{a}\n{b}")}\n]";
                    return true;
                }
                List<(string @namespace, string itemName)> Items = [];
                foreach (var kvp in CustomItemManager.items)
                {
                    var @namespace = kvp.Key;
                    var @item = kvp.Value;

                    if (@namespace.PluginNamespace == targetPluginSpace)
                    {
                        Items.Add((@namespace.ToString(), @item.Name));
                    }
                }
                if (Items.Count == 0)
                {
                    response = $"No items could be found in {targetPluginSpace}:*\nList of namespaces registered: [\n{CustomItemManager.items.Keys.Select(ns => ns.PluginNamespace).Distinct().Aggregate((a, b) => $"{a}\n{b}")}\n]";
                    return false;
                }
                response = $"{Items.Count} items were found in {targetPluginSpace}:*\nList of items: [\n{Items.Select(tuple => $"{tuple.@namespace} => {tuple.itemName}").Aggregate((a, b) => $"{a}\n{b}")}\n]";
                return true;
            }
            var list = CustomItemManager.items.Keys.Select(ns => ns.PluginNamespace).Distinct();
            response = $"{list.Count()} Plugin Namespaces have been registerd.\nUse cbci list <Namespace> for items in one of the namespaces found below. Or use cbci list {PrintAllItemsFlag} to print all items.\nList of Plugin Namespaces: [\n{list.Aggregate((a, b) => $"{a}\n{b}")}\n]";
            return true;
        }
    }
}