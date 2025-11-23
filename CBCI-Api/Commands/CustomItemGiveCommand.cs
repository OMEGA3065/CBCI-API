using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using CustomItemLib.API;
using LabApi.Features.Wrappers;
using Utils;

namespace CustomItemLib.Commands
{
    [CommandHandler(typeof(CustomItemCommand))]
    public class CustomItemGiveCommand : ICommand, IUsageProvider
    {
        public string Command { get; } = "give";
        public string[] Aliases { get; } = new string[] { "g" };
        public string Description { get; } = "Gives a player a custom item.";
        public string[] Usage { get; } = new string[] { "%ItemNamespace%", "%Players%" };

        private bool TryGetRAPlayerIds(ArraySegment<string> arguments, int startIndex, out List<ReferenceHub> hubs)
        {
            hubs = RAUtils.ProcessPlayerIdOrNamesList(arguments, startIndex, out _);
            return hubs != null;
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<Player> targetPlayers = [];
            if (Player.TryGet(sender, out var runningPlayer) && arguments.Count == 1)
            {
                targetPlayers.Add(runningPlayer);
            }
            else if (arguments.Count < 2 && TryGetRAPlayerIds(arguments, 1, out var hubs))
            {
                targetPlayers.AddRange(hubs.Select(h => Player.TryGet(sender, out var player) ? player : null).OfType<Player>());
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

            int count = 0;
            foreach (var player in targetPlayers)
            {
                if (item.TryGiveItem(player))
                    count++;
            }

            response = $"Item \"{item.Name}\"[{itemNamespace}] was given to {count} players.";
            return true;
        }
    }
}