using System;
using Populus.Core.Utils;
using Populus.Core.World.Objects.Events;
using Populus.Core.World.Objects;
using System.Collections.Generic;
using System.Linq;
using ActionMgr = Populus.ActionManager.ActionManager;
using Populus.ActionManager.Actions;

namespace Populus.GroupBot.Chat
{
    [ChatCommandKey("loot")]
    public class LootCommand : IChatCommand
    {
        private const float MAX_LOOT_DISTANCE = 40.0f;

        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Any loot command must come from the leader of the group
            if (chat.SenderGuid != botHandler.Group.Leader.Guid) return;

            // TODO: Maybe add subcommand objects?

            // Loot all corpses that we can when the sub command is "all"
            if (chat.MessageTokenized.Length > 1 && chat.MessageTokenized[1].ToLower() == "all")
            {
                var lootList = FindCloseLootables(botHandler);
                foreach (var lootable in lootList)
                    Loot(botHandler, lootable);
                return;
            }


            // Get the leaders target
            var leaderObj = botHandler.BotOwner.GetPlayerByGuid(botHandler.Group.Leader.Guid);
            if (leaderObj == null) return;
            // TODO: Only works if a unit, what if it's a GameObject?
            var target = botHandler.BotOwner.GetUnitByGuid(leaderObj.TargetGuid);
            if (target == null) return;

            // Get the object that we were told to loot.
            var lootObject = botHandler.BotOwner.GetWorldObjectByGuid(target.Guid);
            if (lootObject == null)
            {
                botHandler.BotOwner.ChatParty($"That target does not exist, I can't loot that.");
                return;
            }

            // Make sure we are close enough to loot the object
            float dist = botHandler.BotOwner.DistanceFrom(lootObject.Position);
            if (dist > MAX_LOOT_DISTANCE)
            {
                botHandler.BotOwner.ChatParty($"That object is too far away. I only loot objects within {MAX_LOOT_DISTANCE} yards. That object is {dist.ToNearestInt()} yards away.");
                return;
            }

            // If the target is not dead, we can't loot it.
            // TODO: Only if a unit, what if it's a GameObject?
            if (!target.IsDead)
            {
                botHandler.BotOwner.ChatParty($"That target is not dead, I can't loot that.");
                return;
            }

            // Loot the object
            Loot(botHandler, target);
        }

        /// <summary>
        /// Find lootables that within range of us
        /// </summary>
        /// <returns></returns>
        private List<WorldObject> FindCloseLootables(GroupBotHandler botHandler)
        {
            List<WorldObject> lootables = new List<WorldObject>();

            // Check for any dead units and see if we can loot them
            var units = botHandler.BotOwner.GetUnitsInRange(MAX_LOOT_DISTANCE).Where(u => u.IsDead);
            lootables.AddRange(units);

            // Check for any gameobjects that are lootable
            var GOs = botHandler.BotOwner.GetGameObjectsInRange(MAX_LOOT_DISTANCE).Where(go => go.CanInteract);
            lootables.AddRange(GOs);

            return lootables;
        }

        /// <summary>
        /// Attempts to loot a world object
        /// </summary>
        /// <param name="wo">World Object to loot</param>
        private void Loot(GroupBotHandler botHandler, WorldObject wo)
        {
            var actionQueue = ActionMgr.GetActionQueue(botHandler.BotOwner.Guid);
            actionQueue.Add(new MoveTowardsObject(botHandler.BotOwner, wo, 1.0f));
            actionQueue.Add(new LootObject(botHandler.BotOwner, wo));
        }
    }
}
