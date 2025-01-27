using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class HolyPrice : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Price");
            Description.SetDefault("30% decreased attack speed");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().AttackSpeed -= 0.30f;
        }
    }
}