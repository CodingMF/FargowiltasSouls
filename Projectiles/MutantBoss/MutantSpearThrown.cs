﻿using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantSpearThrown : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Penetrator");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 1;
            Projectile.scale = 1.3f;
            Projectile.alpha = 0;
            CooldownSlot = 1;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
        }

        protected float scaletimer;
        public override void AI()
        {
            if (--Projectile.localAI[0] < 0)
            {
                Projectile.localAI[0] = 4;
                if ((Projectile.ai[1] == 0 || FargoSoulsWorld.MasochistModeReal) && Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MutantSphereSmall>(), Projectile.damage, 0f, Projectile.owner, Projectile.ai[0]);
            }

            if (Projectile.localAI[1] == 0f)
            {
                Projectile.localAI[1] = 1f;
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            scaletimer++;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), target.Center + Main.rand.NextVector2Circular(100, 100), Vector2.Zero, ModContent.ProjectileType<PhantasmalBlast>(), 0, 0f, Projectile.owner);
            if (FargoSoulsWorld.EternityMode)
            {
                target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 100;
                target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
            }
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/MutantBoss/MutantEye_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int rect1 = glow.Height / Main.projFrames[Projectile.type];
            int rect2 = rect1 * Projectile.frame;
            Rectangle glowrectangle = new Rectangle(0, rect2, glow.Width, rect1);
            Vector2 gloworigin2 = glowrectangle.Size() / 2f;
            Color glowcolor = Color.Lerp(new Color(51, 255, 191, 0), Color.Transparent, 0.82f);
            Color glowcolor2 = Color.Lerp(new Color(194, 255, 242, 0), Color.Transparent, 0.6f);
            glowcolor = Color.Lerp(glowcolor, glowcolor2, 0.5f + (float)Math.Sin(scaletimer / 7) / 2); //make it shift between the 2 colors
            Vector2 drawCenter = Projectile.Center + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 28);

            for (int i = 0; i < 3; i++) //create multiple transparent trail textures ahead of the projectile
            {
                Vector2 drawCenter2 = drawCenter + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 20).RotatedBy(MathHelper.Pi / 5 - (i * MathHelper.Pi / 5)); //use a normalized version of the projectile's velocity to offset it at different angles
                drawCenter2 -= (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 20); //then move it backwards
                float scale = Projectile.scale;
                scale += (float)Math.Sin(scaletimer / 7) / 7; //pulsate slightly so it looks less static
                Main.EntitySpriteDraw(glow, drawCenter2 - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle),
                    glowcolor, Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 1.25f, SpriteEffects.None, 0);
            }

            for (int i = ProjectileID.Sets.TrailCacheLength[Projectile.type] - 1; i > 0; i--) //scaling trail
            {

                Color color27 = glowcolor;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                float scale = Projectile.scale * (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                scale += (float)Math.Sin(scaletimer / 7) / 7; //pulsate slightly so it looks less static
                Vector2 value4 = Projectile.oldPos[i] - (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 14);
                Main.EntitySpriteDraw(glow, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
                    Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 1.25f, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}