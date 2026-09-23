using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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
		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
		{
			if (ChosenClass == 0) // Melee
			{
				return new[] { new Item(ItemID.CopperBroadsword) };
			}
			else if (ChosenClass == 1) // Ranged
			{
				Item bow = new Item(ItemID.CopperBow);
				Item arrows = new Item(ItemID.WoodenArrow, 100);
				return new[] { bow, arrows };
			}
			else if (ChosenClass == 2) // Magic
			{
				Item staff = new Item(ItemID.AmethystStaff);
				Item manaCrystal = new Item(ItemID.ManaCrystal, 1);
				return new[] { staff, manaCrystal };
			}
			else if (ChosenClass == 3) // Summoner
			{
				Item birdStaff = new Item(ItemID.BabyBirdStaff);
				Item whip = new Item(ItemID.BlandWhip);
				return new[] { birdStaff, whip };
			}

			return Enumerable.Empty<Item>();
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
