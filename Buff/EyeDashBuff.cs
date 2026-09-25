using Terraria;
using Terraria.ModLoader;

namespace ClassTree.Buff
{
    public class EyeDashBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<ClassTreePlayer>().hasTearDash = true;
        }
    }
}