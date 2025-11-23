using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using CustomItemLib.API;
using LabApi.Features.Wrappers;
using Utils;

namespace CustomItemLib.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class CustomItemCommand : ParentCommand, IUsageProvider
    {
        public override string Command { get; } = "cbci";

        public override string[] Aliases { get; } = new string[] { "ci" };

        public override string Description { get; } = "Gives a player a custom item.";

        public string[] Usage { get; } = new string[] { "%SubCommand%" };

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new CustomItemGiveCommand());
        }

        public CustomItemCommand()
        {
            LoadGeneratedCommands();
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission([PlayerPermissions.GivingItems]))
            {
                response = "You don't have the required permission to use this command.";
                return false;
            }

            response = $"Use one of the following SubCommands: [{Commands.Keys.Aggregate((a, b) => $"{a}, {b}")}]";
            return false;
        }
    }
}