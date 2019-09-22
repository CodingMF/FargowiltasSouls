﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using ThoriumMod;
using Terraria.Localization;
using SacredTools;

namespace FargowiltasSouls.Items.Accessories.Enchantments.SoA
{
    public class FallenPrinceEnchant : ModItem
    {
        private readonly Mod soa = ModLoader.GetMod("SacredTools");

        public override bool Autoload(ref string name)
        {
            return ModLoader.GetMod("SacredTools") != null;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fallen Prince Enchantment");
            Tooltip.SetDefault(
@"'Give up your heritage, gain power'
Hitting enemies has a chance to summon an energy blade behind you (Max of 5).
Every newly acquired blade increases the damage of pre-existing ones by 20%
Pressing [Ability] will launch the blades at nearby enemies
Effects of Novaniel's Resolve");
            DisplayName.AddTranslation(GameCulture.Chinese, "堕落王子魔石");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"'舍弃继承权, 换取力量'
攻击敌人有概率在身后召唤一柄能量剑(最多5柄)
每柄新获得的剑会增加20%先前存在剑的伤害
按[特殊能力]键将会把剑射向附近敌人
拥有诺瓦尼尔的决心的效果");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = 11;
            item.value = 350000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!Fargowiltas.Instance.SOALoaded) return;

            ModdedPlayer modPlayer = player.GetModPlayer<ModdedPlayer>();

            //set bonus
            modPlayer.NovanielArmor = true;

            //novaniels resolve
            ModLoader.GetMod("SacredTools").GetItem("NovanielResolve").UpdateAccessory(player, hideVisual);
        }

        private readonly string[] items =
        {
            "FallenPrinceHelm",
            "FallenPrinceChest",
            "FallenPrinceBoots",
            "NovanielResolve",
            "CosmicDesolation",
            "LunaticsGamble",
            "Dawnfall",
            "FlariumDisc",
            "AsthralSaber",
            "AsthralKnives"
        };

        public override void AddRecipes()
        {
            if (!Fargowiltas.Instance.SOALoaded) return;

            ModRecipe recipe = new ModRecipe(mod);

            foreach (string i in items) recipe.AddIngredient(soa.ItemType(i));

            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
