using System;
using Populus.Core.Utils;
using Populus.Core.World.Objects.Events;

namespace Populus.GroupBot.Chat
{
    [ChatCommandKey("attack")]
    public class AttackCommand : IChatCommand
    {
        private const float MAX_ATTACK_DISTANCE = 40.0f;

        public void ProcessCommand(GroupBotHandler botHandler, ChatEventArgs chat)
        {
            if (botHandler == null) throw new ArgumentNullException("botHandler");
            if (chat == null) throw new ArgumentNullException("chat");

            // Any attack command must come from the leader of the group
            if (chat.SenderGuid != botHandler.Group.Leader.Guid) return;

            // Get the leaders target
            var leaderObj = botHandler.BotOwner.GetPlayerByGuid(botHandler.Group.Leader.Guid);
            if (leaderObj == null) return;
            var target = botHandler.BotOwner.GetUnitByGuid(leaderObj.TargetGuid);
            if (target == null) return;

            // If the target is too far away, send a message
            float dist = botHandler.BotOwner.DistanceFrom(target.Position);
            if (dist > MAX_ATTACK_DISTANCE)
            {
                botHandler.BotOwner.ChatParty($"That target is too far away. I only attack targets within {MAX_ATTACK_DISTANCE} yards. That target is {dist.ToNearestInt()} yards away.");
                return;
            }

            // Attack!
            botHandler.CombatHandler.StartAttack(target);
        }
    }
}
