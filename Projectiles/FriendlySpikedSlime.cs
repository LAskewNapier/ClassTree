using System;
using Humanizer;
using Microsoft.Build.Construction;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ClassTree.Projectiles
{
    public class FriendlySpikedSlime : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 21;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.owner == Main.myPlayer && Main.GameUpdateCount % 30 == 0)
                {
                    Main.NewText($"onGround check: velY={Projectile.velocity.Y:F2} pos={Projectile.Center} tileCheck={((int)(Projectile.Center.X / 16f), (int)((Projectile.Bottom.Y + 4f) / 16f))}");
                }

            player.AddBuff(ModContent.BuffType<Buff.FriendlySpikedSlimeBuff>(), 1800);
            // keep the minion alive as long as the player has the buff
            if (player.dead || !player.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 10f)
            {
                Projectile.velocity.Y = 10f;
                
            }
            Projectile.tileCollide = true;
            int attackTarget = -1;
            Projectile.Minion_FindTargetInRange(700, ref attackTarget, false);

            
            bool onGround = false;
            int tileX = (int)(Projectile.Center.X / 16f);
            int tileY = (int)((Projectile.Bottom.Y + 4f) / 16f);

            if (tileX >= 0 && tileX < Main.maxTilesX && tileY >= 0 && tileY < Main.maxTilesY)
            {
                Tile tile = Main.tile[tileX, tileY];
                onGround = tile.HasTile && Main.tileSolid[tile.TileType];
            }
            if (onGround)
            {
                Projectile.ai[1]++;
                if (onGround && Projectile.ai[1] > 20)
                {
                    Projectile.ai[1] = 0;
                    Projectile.velocity.Y = -7f;

                    float dirX;
                    if (attackTarget != -1)
                    {
                        dirX = Main.npc[attackTarget].Center.X > Projectile.Center.X ? 1f : -1f;

                    }
                    else
                    {
                        dirX = player.Center.X > Projectile.Center.X ? 1f : -1f;
                    }
                    Projectile.velocity.X = dirX * 3f;
                }
            }
            else
            {
                Projectile.ai[1] = 0;
            }





            //combat behavior
            if (attackTarget != -1)
            {
                NPC target = Main.npc[attackTarget];

                if (target.Center.X > Projectile.Center.X)
                {
                    Projectile.direction = Projectile.spriteDirection = 1;
                }
                else
                {
                    Projectile.direction = Projectile.spriteDirection = -1;
                }

                Projectile.ai[0]++;
                if (Projectile.ai[0] >= 60)
                {
                    Projectile.ai[0] = 0;

                    Vector2 toTarget = target.Center - Projectile.Center;
                    toTarget.Y -= 40f;

                    Vector2 shootVelocity = Vector2.Normalize(toTarget) * 12f;
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromAI(),
                        Projectile.Center,
                        shootVelocity,
                        ModContent.ProjectileType<Projectiles.FriendlySpikedSlimeSpike>(),
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner
                    );
                }
                Projectile.friendly = true;
            }
            else
            {
                Vector2 idleProition = player.Center;
                idleProition.Y -= 48f;
                float distance = Vector2.Distance(Projectile.Center, idleProition);

                if (distance > 800f)
                {
                    Projectile.position = idleProition;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.netUpdate = true;
                } 
                else if (distance > 20f)
                {
                    Vector2 direction = Vector2.Normalize(idleProition - Projectile.Center) * 3f;
                    Projectile.velocity.X = (Projectile.velocity.X * 20f + direction.X) / 21f;
                }
                else
                {
                    Projectile.velocity.X *= 0.9f;
                }
                Projectile.friendly = false;
            }
            Projectile.rotation = Projectile.velocity.X * 0.05f;

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = 0f;

            }
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X * 0.5f;
            }
            return false;
        }
    }
}