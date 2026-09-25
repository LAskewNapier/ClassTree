using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using ClassTree.Items;

namespace ClassTree.GlobalNPCs
{
    public class KingSlimeGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.KingSlime)
            {
                // Add a drop rule for the Bested Crown with a 100% chance
                npcLoot.Add(ItemDropRule.ByCondition( new BestedCrownCondition(), ModContent.ItemType<BestedCrown>(), 1));
            }
        }
    }

    public class BestedCrownCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            // Check if the player has already received the Bested Crown
            var player = info.player.GetModPlayer<ClassTreePlayer>();
            return !player.hasReceivedCrown;
        }
        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "Drops only if you haven't received the Bested Crown yet.";
        }
    }

        public class EyeofCthulhuGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
                // Add a drop rule for the Bested Crown with a 100% chance
                npcLoot.Add(ItemDropRule.ByCondition( new BloodyTearCondition(), ModContent.ItemType<BloodyTear>(), 1));
            }
        }
    }

    public class BloodyTearCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            // Check if the player has already received the Bloody Tear
            var player = info.player.GetModPlayer<ClassTreePlayer>();
            return !player.hasReceivedTear;
        }
        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "Drops only if you haven't received the Bloody Tear yet.";
        }
    }
}