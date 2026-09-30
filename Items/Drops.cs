using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using ClassTree.Items;
using ClassTree.GlobalNPCs;
using System.Text;

namespace ClassTree.GlobalItems
{
    public class VanillaBagLoot : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.type == ItemID.EyeOfCthulhuBossBag)
            {
                itemLoot.Add(ItemDropRule.ByCondition( 
                        new BloodyTearCondition(), 
                        ModContent.ItemType<BloodyTear>(), 
                        1));
            }
            else if (item.type == ItemID.KingSlimeBossBag)
            {
                itemLoot.Add(ItemDropRule.ByCondition( 
                        new BestedCrownCondition(), 
                        ModContent.ItemType<BestedCrown>(), 
                        1));
            }
            else if (item.type == ItemID.EaterOfWorldsBossBag)
            {
                itemLoot.Add(ItemDropRule.ByCondition( 
                        new EvilBiomoCondition(), 
                        ModContent.ItemType<EatersTooth>(), 
                        1));
            }
            else if (item.type == ItemID.BrainOfCthulhuBossBag)
            {
                itemLoot.Add(ItemDropRule.ByCondition( 
                        new EvilBiomoCondition(), 
                        ModContent.ItemType<BabyCreeper>(), 
                        1));
            }
        }
    }
}

namespace ClassTree.GlobalNPCs
{
    public class KingSlimeGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.KingSlime)
            {
                // Add a drop rule for the Bested Crown with a 100% chance
                ItemDropRule.ByCondition( 
                    new BestedCrownCondition(), 
                    ModContent.ItemType<BestedCrown>(), 
                    1);
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

                ItemDropRule.ByCondition( 
                    new BloodyTearCondition(), 
                    ModContent.ItemType<BloodyTear>(), 
                    1);
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

    public class EvilBiomoGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.EaterofWorldsTail)
            {
                // Add a drop rule for the Eaters Tooth with a 100% chance
                npcLoot.Add(ItemDropRule.ByCondition( new EvilBiomoCondition(), ModContent.ItemType<EatersTooth>(), 1));
            }
            if (npc.type == NPCID.BrainofCthulhu)
            {
                // Add a drop rule for the Baby Creeper with a 100% chance
                npcLoot.Add(ItemDropRule.ByCondition( new EvilBiomoCondition(), ModContent.ItemType<BabyCreeper>(), 1));
            }
        }
    }

    public class EvilBiomoCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            // Check if the player has already received the Bloody Tear
            var player = info.player.GetModPlayer<ClassTreePlayer>();
            return !player.hasReceivedEvil;
        }
        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "Drops only if you haven't received an evil biomo skill tree drop yet.";
        }
    }
}