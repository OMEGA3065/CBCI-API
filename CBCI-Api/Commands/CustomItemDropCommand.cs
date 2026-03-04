using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using CustomItemLib.API;
using LabApi.Features.Wrappers;
using Utils;

namespace CustomItemLib.Commands
{
    [CommandHandler(typeof(CustomItemCommand))]
    public class CustomItemDropCommand : ICommand, IUsageProvider
    {
        public string Command { get; } = "drop";
        public string[] Aliases { get; } = new string[] { "d" };
        public string Description { get; } = "Drops a custom item on specified players.";
        public string[] Usage { get; } = new string[] { "%ItemNamespace%", "[CountPerPlayer]", "%Players%" };

        private bool TryGetRAPlayerIds(ArraySegment<string> arguments, int startIndex, out List<ReferenceHub> hubs)
        {
            hubs = RAUtils.ProcessPlayerIdOrNamesList(arguments, startIndex, out _);
            return hubs != null;
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems))
            {
                response = "You don't have the required permission to use this command.";
                return false;
            }
            
            List<Player> targetPlayers = [];
            if (arguments.Count >= 3 && TryGetRAPlayerIds(arguments, 2, out var hubs))
            {
                targetPlayers.AddRange(hubs.Select(h => Player.Get(h)));
            }
            else if (Player.TryGet(sender, out var runningPlayer))
            {
                targetPlayers.Add(runningPlayer);
            }
            else
            {
                response = "You must either be a player to use this command or specify valid players to give the item to!";
                return false;
            }

            if (!ItemNamespace.TryGet(arguments.At(0), out var itemNamespace))
            {
                response = "Invalid item namespace format. Expected format: 'plugin_namespace:item_identifier'";
                return false;
            }

            if (!CustomItemManager.TryGetItem(itemNamespace, out var item))
            {
                response = $"Could not find an item under the namespace of {itemNamespace}. Please make sure that item exists.";
                return false;
            }
            
            if (!int.TryParse(arguments.At(1), out int amount))
            {
                response = "Invalid item namespace format. Expected format: 'plugin_namespace:item_identifier'";
                return false;
            }

            int count = 0;
            foreach (var player in targetPlayers)
            {
                bool any = false;
                for (int i = 0; i < amount; i++)
                {
                    if (item.TrySpawn(player.Position))
                        any = true;
                }

                if (any) count++;
            }

            response = $"Item \"{item.Name}\"[{itemNamespace}] was spawned at {count} players {amount} times.";
            return true;
        }
    }
}