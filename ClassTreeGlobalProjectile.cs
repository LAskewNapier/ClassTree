using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using ClassTree.Items;
using Microsoft.Build.Evaluation;

namespace ClassTree.GlobalProjectiles
{
    public class ClassTreeGlobalProjectile : GlobalProjectile
    {
        // Ranged: 10% chance for ammo to bounce
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Make sure the projectile has a valid owner
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                return;
            }

            var modPlayer = Main.player[projectile.owner].GetModPlayer<ClassTreePlayer>();

            //Only apply if the crown is unlocked and shared node is chosen
            if (modPlayer.UnlockedSkills.Contains("Crown_Unlocked") && modPlayer.UnlockedSkills.Contains("Slime_Path_A") 
            && modPlayer.ChosenClass == 1 && projectile.DamageType == DamageClass.Ranged && Main.rand.NextFloat() < 0.10f) // 10% chance to apply the effect
            {
                projectile.velocity *= -1f; // Reverse the projectile's velocity to make it bounce
                projectile.timeLeft += 60; // Extend the projectile's lifetime by 1 second (60 ticks)
                projectile.penetrate = -1; // prevents the projectile from being destroyed on impact
                projectile.localNPCImmunity[target.whoAmI] = 10; // Prevents the projectile from hitting the same NPC for 10 ticks (1/6th of a second)
                projectile.netUpdate = true; // Sync the projectile's state with other clients
            }


        }
        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                return true;
            }

            if (projectile.DamageType != DamageClass.Ranged)
            {
                return true;
            }
            var modPlayer = Main.player[projectile.owner].GetModPlayer<ClassTreePlayer>();

            if (!modPlayer.UnlockedSkills.Contains("Crown_Unlocked") || !modPlayer.UnlockedSkills.Contains("Slime_Path_A") || modPlayer.ChosenClass != 1)
            {
                return true;
            }

            if (Main.rand.NextFloat() >= 0.10f)
            {
                return true;
            }

            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }

            projectile.timeLeft += 60;

            projectile.netUpdate = true;
            return false;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.type != ProjectileID.BabySlime || !projectile.minion)
            {
                return;
            }

            if (projectile.owner < 0 || projectile.owner >= Main.maxPlayers)
            {
                return;
            }
            var modPlayer = Main.player[projectile.owner].GetModPlayer<ClassTreePlayer>();
            if (!modPlayer.UnlockedSkills.Contains("Crown_Unlocked") || !modPlayer.UnlockedSkills.Contains("Slime_Path_A"))
            {
                return;
            }
            int newproj = Projectile.NewProjectile(
                source, 
                projectile.Center,
                projectile.velocity,
                ModContent.ProjectileType<Projectiles.FriendlySpikedSlime>(),
                projectile.damage,
                projectile.knockBack,
                projectile.owner
            );

            Main.projectile[newproj].originalDamage = projectile.originalDamage;
            Main.projectile[newproj].minionSlots = projectile.minionSlots;
            Main.projectile[newproj].timeLeft = projectile.timeLeft;

            projectile.active = false;
            projectile.netUpdate = true;

        }
        


    }
}