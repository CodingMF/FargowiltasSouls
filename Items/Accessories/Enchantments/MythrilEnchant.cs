using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class MythrilEnchant : ModItem
    {
        private readonly Mod thorium = ModLoader.GetMod("ThoriumMod");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mythril Enchantment");
            Tooltip.SetDefault(
@"20% increased weapon use speed
Taking damage temporarily removes this weapon use speed increase
'You feel the knowledge of your weapons seep into your mind'");
            DisplayName.AddTranslation(GameCulture.Chinese, "秘银魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"你感觉你对武器的知识渗透到脑海中'
增加25%武器使用速度");
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine tooltipLine in list)
            {
                if (tooltipLine.mod == "Terraria" && tooltipLine.Name == "ItemName")
                {
                    tooltipLine.overrideColor = new Color(157, 210, 144);
                }
            }
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = 5;
            item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoPlayer>().MythrilEnchant = true;
            if (SoulConfig.Instance.GetValue(SoulConfig.Instance.MythrilSpeed))
                player.GetModPlayer<FargoPlayer>().AttackSpeed += .2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("FargowiltasSouls:AnyMythrilHead");
            recipe.AddIngredient(ItemID.MythrilChainmail);
            recipe.AddIngredient(ItemID.MythrilGreaves);
            //mythril sword
            //mythril repeater
            //flintlock pistol
            recipe.AddIngredient(ItemID.LaserRifle);
            recipe.AddIngredient(ItemID.ClockworkAssaultRifle);
            recipe.AddIngredient(ItemID.Gatligator);
            recipe.AddIngredient(ItemID.OnyxBlaster);

            recipe.AddTile(TileID.CrystalBall);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
