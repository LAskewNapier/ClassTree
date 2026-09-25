using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

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
                target.AddBuff(BuffID.Slow, 180); // Apply the Slow debuff for 3 seconds (180 ticks)
            }
        }

        public override void ModifyShootStats( Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            var modPlayer = player.GetModPlayer<ClassTreePlayer>();
    		// Safety check: ensure we have a valid ModPlayer instance
    		if (!modPlayer.UnlockedSkills.Contains("Eye_Unlocked") || !modPlayer.UnlockedSkills.Contains("Eye_Path_B"))
        		return;

    		// Class check: only Ranged
    		if (modPlayer.ChosenClass != 1)
        		return;

    		// Health check: only active at 25% HP or lower
    		if (player.statLife > player.statLifeMax2 * 0.25f)
        		return;

    		// Apply the 5% velocity boost
    		velocity *= 3f;
		}

        public override float UseTimeMultiplier(Item item, Player player)
        {
            var modPlayer = player.GetModPlayer<ClassTreePlayer>();
            if (!modPlayer.UnlockedSkills.Contains("Eye_Unlocked") || !modPlayer.UnlockedSkills.Contains("Eye_Path_B"))
        		return 1f;

    		// Class check: only Ranged
    		if (modPlayer.ChosenClass != 2)
        		return 1f;

            // Health check: only active at 25% HP or lower
    		if (player.statLife > player.statLifeMax2 * 0.25f)
        		return 1f;

            return 0.5f;

        }

        public override float UseAnimationMultiplier(Item item, Player player)
        {
            var modPlayer = player.GetModPlayer<ClassTreePlayer>();
            if (!modPlayer.UnlockedSkills.Contains("Eye_Unlocked") || !modPlayer.UnlockedSkills.Contains("Eye_Path_B"))
        		return 1f;

    		// Class check: only Ranged
    		if (modPlayer.ChosenClass != 2)
        		return 1f;

            // Health check: only active at 25% HP or lower
    		if (player.statLife > player.statLifeMax2 * 0.25f)
        		return 1f;

            return 0.5f;

        }
    }
}