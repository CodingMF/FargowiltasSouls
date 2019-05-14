using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Fused : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Fused");
            Description.SetDefault("You're going out with a bang");
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoPlayer>().Fused = true;

            if (player.buffTime[buffIndex] == 2)
            {
                player.immune = false;
                player.immuneTime = 0;
                //Projectile.NewProjectile(player.Center, Vector2.Zero, ProjectileID.Explosives, 400, 10f, Main.myPlayer);
                int damage = (player.statLifeMax2 / 2) + Main.rand.Next(-player.statLifeMax2 / 8, player.statLifeMax2 / 4);
                Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("FusionBomb"), damage / 4, 4f, Main.myPlayer);
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " was blown to bits."), damage, 0, false, false, true);
            }
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return true;
        }
    }
}