using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ClassTree.Items
{
    public class BloodyTear : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Expert;
            Item.accessory = false; // This item is not an accessory
            Item.useStyle = ItemUseStyleID.HoldUp; // Use style for holding up the item
            Item.useTime = 20; // Time it takes to use the item
            Item.useAnimation = 20; // Animation time for using the item
            Item.consumable = true; // This item is consumable
        }

        public override bool? UseItem(Player player)
        {
            // Set the hasReceivedCrown flag to true when the item is used
            var modPlayer = player.GetModPlayer<ClassTreePlayer>();

            if (modPlayer.UnlockedSkills.Contains("Tear_Unlocked"))
            {
                // Player has already used the Bloody Tear, do not allow further use
                Main.NewText("You have already used the Bloody Tear and unlocked the skill tree.");
                return false;
            }

            modPlayer.UnlockedSkills.Add("Tear_Unlocked");
            Main.NewText("You have unlocked the Tear skill tree nodes!", 255, 215, 0); // Display a message in gold color
            return true; // Allow the item to be used
        }

        public override bool OnPickup(Player player)
        {
            // Set the hasReceivedTear flag to true when the item is picked up
            var modPlayer = player.GetModPlayer<ClassTreePlayer>();
            modPlayer.hasReceivedTear = true;
            return true; // Allow the item to be picked up
        }
    }
}