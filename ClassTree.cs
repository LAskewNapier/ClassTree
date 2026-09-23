using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace ClassTree
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class ClassTree : Mod
	{

	}

	public class ClassTreePlayer : ModPlayer
	{
		public int ChosenClass = -1; // -1 = None, 0 = Melee, 1 = Ranged, 2 = Magic, 3 = Summoner

		public override void SaveData(TagCompound tag)
		{
			tag["ChosenClass"] = ChosenClass;
		}
		public override void LoadData(TagCompound tag)
		{
			ChosenClass = tag.GetInt("ChosenClass");
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
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.CopperShortsword) ; 
			}
			else if (ChosenClass == 1) // Ranged
			{
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.CopperBow);
				Player.QuickSpawnItem(Player.GetSource_Misc("ClassTree"), ItemID.WoodenArrow, 100);
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
