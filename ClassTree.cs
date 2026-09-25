using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace ClassTree
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class ClassTree : Mod
	{
		public static ModKeybind OpenSkillTreeKeybind { get; private set; }

		public override void Load()
		{
			if (Main.dedServ)
			{
				return;
			}
			Main.NewText("this part works");
			OpenSkillTreeKeybind = KeybindLoader.RegisterKeybind(this, "Open Skill Tree", "P");
		}

		public override void Unload()
		{
			OpenSkillTreeKeybind = null;
		}
	}

	public class ClassTreePlayer : ModPlayer
	{
		public int ChosenClass = -1; // -1 = None, 0 = Melee, 1 = Ranged, 2 = Magic, 3 = Summoner
		public bool hasReceivedCrown = false; // Track if the player has received the Bested Crown
		public bool hasReceivedTear = false; // Tracks if the player has received the Bloody Tear

		public HashSet<string> UnlockedSkills = new HashSet<string>();

		public bool hasTearDash = false;
		public override void SaveData(TagCompound tag)
		{
			tag["ChosenClass"] = ChosenClass;
			tag["hasReceivedCrown"] = hasReceivedCrown;
			tag["hasReceivedTear"] = hasReceivedTear;
			tag["UnlockedSkills"] = UnlockedSkills.ToList();
		}
		public override void LoadData(TagCompound tag)
		{
			ChosenClass = tag.GetInt("ChosenClass");
			hasReceivedCrown = tag.GetBool("hasReceivedCrown");
			hasReceivedTear = tag.GetBool("hasReceivedTear");
			UnlockedSkills = new HashSet<string>(tag.GetList<string>("UnlockedSkills"));
		}        
		public override void OnEnterWorld()
        {
			if (ChosenClass == -1)
			{
				for (int i = 0; i < Player.inventory.Length; i++)
				{
					if (Player.inventory[i].type != ItemID.None)
					{
						if (Player.inventory[i].type == ItemID.CopperShortsword || Player.inventory[i].type == ItemID.CopperBow || Player.inventory[i].type == ItemID.AmethystStaff || Player.inventory[i].type == ItemID.BabyBirdStaff)
						{
							Player.inventory[i].TurnToAir();
						}
					}
				}
				IngameFancyUI.OpenUIState(new UI.ClassSelectionUI());
			}
        }
		public void GiveStartingItems()
		{
			if (ChosenClass == 0) // Melee
			{
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.CopperBroadsword); 
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.SlimeCrown, 5);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.Zenith);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.SuspiciousLookingEye, 5);
			}
			else if (ChosenClass == 1) // Ranged
			{
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.CopperBow);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.WoodenArrow, 100);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.VortexBeater);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.MusketBall, 1000);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.SlimeCrown, 5);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.SuspiciousLookingEye, 5);

			}
			else if (ChosenClass == 2) // Magic
			{
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.AmethystStaff);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.ManaCrystal, 20);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.SlimeCrown, 5);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.SuspiciousLookingEye, 5);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.LunarFlareBook);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.NebulaHelmet);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.NebulaBreastplate);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.NebulaLeggings);
			}
			else if (ChosenClass == 3) // Summoner
			{
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.BabyBirdStaff);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.BlandWhip);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.SlimeStaff);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.RainbowWhip);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.SlimeCrown, 5);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.SuspiciousLookingEye, 5);
			}
		}

        public override void ResetEffects()
        {
            hasTearDash = false;
        }

        public override void PostUpdateRunSpeeds()
        {
            if (UnlockedSkills.Contains("Move_Speed"))
			{
				Player.moveSpeed += 0.05f; // Increase move speed by 5%
			}
        }

		public override void PostUpdateEquips()
		{
			ApplyStarterEffects();
			ApplyKingSlimeEffects();
			ApplyTearEffects();

			if (hasTearDash)
			{
				Player.dashType = 2;
			}
		}

		private void ApplyStarterEffects()
		{
			if (UnlockedSkills.Contains("Damage"))
			{
				Player.GetDamage(DamageClass.Generic) += 0.05f; // Increase damage by 5%
			}
		}

		private void ApplyKingSlimeEffects()
		{
			if (UnlockedSkills.Contains("Crown_Unlocked") && UnlockedSkills.Contains("Slime_Path_A") && ChosenClass == 2)
			{
				Player.GetCritChance(DamageClass.Magic) += 3f; // Increase magic crit chance by 3%
			}
		}

		private void ApplyTearEffects()
		{
			if (!UnlockedSkills.Contains("Eye_Unlocked"))
			{
				return;
			}
			bool lowhp = Player.statLife <= Player.statLifeMax2 * 0.25f;

			//add dash to ungrade selection
			if (UnlockedSkills.Contains("Eye_Path_A") && (ChosenClass == 0 || ChosenClass == 3))
			{
				Player.AddBuff(ModContent.BuffType<Buff.EyeDashBuff>(), 2);
			}

			if (UnlockedSkills.Contains("Eye_Path_B") && lowhp && (ChosenClass == 0 || ChosenClass == 3))
			{
				Player.GetAttackSpeed(DamageClass.Melee) += 0.05f;
				Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.05f;
			}
		}



        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (UnlockedSkills.Contains("Eye_Unlocked") && UnlockedSkills.Contains("Eye_Path_A") && ChosenClass == 1 && proj.DamageType == DamageClass.Ranged)
			{
				modifiers.CritDamage += 0.05f;
			}
        }



		public override void ModifyMaxStats(out StatModifier Health, out StatModifier Mana)
		{
			Health = StatModifier.Default;
			Mana = StatModifier.Default;

			if (UnlockedSkills.Contains("Health"))
			{
				Health.Base = 20f; // Increase max health by 20
			}
		}
		public override void OnHurt(Player.HurtInfo info)
		{
			ApplySlimeMeleeRetaliation(info);
			ApplyEyeStarDrops(info);
		}

		public void ApplySlimeMeleeRetaliation(Player.HurtInfo info)
		{
			if (ChosenClass != 0) return; 
			if (UnlockedSkills.Contains("Crown_Unlocked") && UnlockedSkills.Contains("Slime_Path_A"))
			{
				int damage = (int)Player.GetTotalDamage(DamageClass.Melee).ApplyTo(10f); 

				for (int i = 0; i < 8; i++)
				{
					Vector2 baseVelocity = new Vector2(8f, 0f); // Base velocity pointing upwards
					Vector2 velocity = baseVelocity.RotatedByRandom(MathHelper.TwoPi); // Rotate the base velocity by 45 degrees for each projectile

					Projectile.NewProjectile(
						Player.GetSource_OnHurt(info.DamageSource), 
						Player.Center, 
						velocity, 
						ModContent.ProjectileType<Projectiles.FriendlySpikedSlimeSpike>(), 
						damage, 
						0f, 
						Player.whoAmI);
				}

			}
		}

		private void ApplyEyeStarDrops(Player.HurtInfo info)
		{
			if (ChosenClass != 2) return;
			if (!UnlockedSkills.Contains("Eye_Unlocked") || !UnlockedSkills.Contains("Eye_Path_A")) return;
			int damage = (int)Player.GetTotalDamage(DamageClass.Magic).ApplyTo(30f);
			for (int i = 0; i < 3; i++)
			{
				Vector2 spawnPos = Player.Center + new Vector2(Main.rand.Next(-20, 200), -400f);
				Vector2 velocity = new Vector2(0f, 12f);

				int star = Projectile.NewProjectile(
					Player.GetSource_OnHurt(info.DamageSource),
					spawnPos,
					velocity,
					ProjectileID.FallingStar,
					damage,
					0f,
					Player.whoAmI);
				
				Main.projectile[star].friendly = true;
				Main.projectile[star].hostile = false;
				Main.projectile[star].DamageType = DamageClass.Magic;
			}
		}


        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (ClassTree.OpenSkillTreeKeybind.JustPressed)
			{
				IngameFancyUI.OpenUIState(new UI.ClassSkillTreeUI());
			}
        }


	}



	public class ClassLockItem : GlobalItem
	{
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            ClassTreePlayer classPlayer = player.GetModPlayer<ClassTreePlayer>();
			if (classPlayer.ChosenClass == -1)
			{
				return;
        	}

			bool isMelee = item.DamageType == DamageClass.Melee;
			bool isRanged = item.DamageType == DamageClass.Ranged;
			bool isMagic = item.DamageType == DamageClass.Magic;
			bool isSummon = item.DamageType == DamageClass.Summon;

			if (!isMelee && !isRanged && !isMagic && !isSummon)
			{
				return;
			}

			bool isCorrectClass = (classPlayer.ChosenClass == 0 && isMelee) ||
								  (classPlayer.ChosenClass == 1 && isRanged) ||
								  (classPlayer.ChosenClass == 2 && isMagic) ||
								  (classPlayer.ChosenClass == 3 && isSummon);
								  
			if (!isCorrectClass)
			{
				damage *= 0f; // Reduce damage by 100% for weapons not matching the chosen class
			}
		}
	}
}
