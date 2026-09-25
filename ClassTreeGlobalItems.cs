using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ClassTree.GlobalItems
{
    public class ClassTreeGlobalItems : GlobalItem
    {
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var modPlayer = player.GetModPlayer<ClassTreePlayer>();

            //Only apply if the crown is unlocked and shared node is chosen
            if (!modPlayer.UnlockedSkills.Contains("Crown_Unlocked"))
            {
                return;
            }
            if (!modPlayer.UnlockedSkills.Contains("Slime_Path_B"))
            {
                return;
            }
            // Check if the item is a weapon
            if (item.damage <= 0)
            {
                return;
            }

            if (Main.rand.NextFloat() < 0.33f) // 33% chance to apply the effect
            {
                target.AddBuff(BuffID.Slimed, 180); // Apply the Slimed debuff for 3 seconds (180 ticks)
            }
        }
    }
}