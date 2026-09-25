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
			}
            if (ChosenClass == -1)
			{
				Main.NewText("this part works");
				IngameFancyUI.OpenUIState(new UI.ClassSelectionUI());
			}
        }
		public void GiveStartingItems()
		{
			Main.NewText("AddStartingItems called with ChosenClass: " + ChosenClass);
			if (ChosenClass == 0) // Melee
			{
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.CopperBroadsword); 
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.SlimeCrown, 5);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.Zenith);
			}
			else if (ChosenClass == 1) // Ranged
			{
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.CopperBow);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.WoodenArrow, 100);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.VortexBeater);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.MusketBall, 1000);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.SlimeCrown, 5);

			}
			else if (ChosenClass == 2) // Magic
			{
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.AmethystStaff);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.ManaCrystal, 1);
			}
			else if (ChosenClass == 3) // Summoner
			{
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.BabyBirdStaff);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.BlandWhip);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.SlimeStaff);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.RainbowWhip);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.SlimeCrown);
			}
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
			if (UnlockedSkills.Contains("Damage"))
			{
				Player.GetDamage(DamageClass.Generic) += 0.05f; // Increase damage by 5%
			}

			if (UnlockedSkills.Contains("Crown_Unlocked") && UnlockedSkills.Contains("Slime_Path_A") && ChosenClass == 2)
			{
				Player.GetCritChance(DamageClass.Magic) += 3f; // Increase magic crit chance by 3%
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
