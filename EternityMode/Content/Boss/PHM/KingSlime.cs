﻿using Fargowiltas.Items.Summons;
using Fargowiltas.NPCs;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Boss.PHM
{
    public class KingSlime : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.KingSlime);

        public int SpikeRainCounter; // Was Counter[0]

        public bool IsBerserk; // Was masoBool[0]
        public bool LandingAttackReady; // Was masoBool[1]
        public bool CurrentlyJumping; // Was masoBool[3]

        public bool DroppedSummon;

        public override bool PreAI(NPC npc)
        {
            EModeGlobalNPC.slimeBoss = npc.whoAmI;
            npc.color = Main.DiscoColor * 0.3f; // Rainbow colour

            // Attack that happens when landing
            if (LandingAttackReady)
            {
                if (npc.velocity.Y == 0f)
                {
                    LandingAttackReady = false;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        /*for (int i = 0; i < 30; i++) //spike spray
                        {
                            Projectile.NewProjectile(new Vector2(npc.Center.X + Main.rand.Next(-5, 5), npc.Center.Y - 15),
                                new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-8, -5)),
                                ProjectileID.SpikedSlimeSpike, npc.damage / 5, 0f, Main.myPlayer);
                        }*/

                        if (npc.HasValidTarget)
                        {
                            Main.PlaySound(SoundID.Item21, Main.player[npc.target].Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    Vector2 spawn = Main.player[npc.target].Center;
                                    spawn.X += Main.rand.Next(-150, 151);
                                    spawn.Y -= Main.rand.Next(600, 901);
                                    Vector2 speed = Main.player[npc.target].Center - spawn;
                                    speed.Normalize();
                                    speed *= IsBerserk ? 10f : 5f;
                                    speed = speed.RotatedByRandom(MathHelper.ToRadians(4));
                                    Projectile.NewProjectile(spawn, speed, ModContent.ProjectileType<SlimeBallHostile>(), npc.damage / 6, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }
                }
            }
            else if (npc.velocity.Y > 0)
            {
                // If they're in the air, flag that the landing attack should be used next time they land
                LandingAttackReady = true;
            }

            if (npc.velocity.Y < 0) // Jumping up
            {
                if (!CurrentlyJumping) // Once per jump...
                {
                    CurrentlyJumping = true;

                    // If player is well above me, jump higher and spray spikes
                    if (npc.HasValidTarget && Main.player[npc.target].Center.Y < npc.position.Y + npc.height - 240)
                    {
                        npc.velocity.Y *= 2f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const float gravity = 0.15f;
                            float time = 90f;
                            Vector2 distance = Main.player[npc.target].Center - npc.Center + Main.player[npc.target].velocity * 30f;
                            distance.X = distance.X / time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            for (int i = 0; i < 15; i++)
                            {
                                Projectile.NewProjectile(npc.Center, distance + Main.rand.NextVector2Square(-1f, 1f),
                                    ModContent.ProjectileType<SlimeSpike>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                }
            }
            else
            {
                CurrentlyJumping = false;
            }

            if ((IsBerserk || npc.life < npc.lifeMax * .5f) && npc.HasValidTarget)
            {
                if (--SpikeRainCounter < 0) // Spike rain
                {
                    SpikeRainCounter = 240;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -12; i <= 12; i++)
                        {
                            Vector2 spawnPos = Main.player[npc.target].Center;
                            spawnPos.X += 110 * i;
                            spawnPos.Y -= 500;
                            Projectile.NewProjectile(spawnPos, (IsBerserk ? 6f : 0f) * Vector2.UnitY,
                                ModContent.ProjectileType<SlimeSpike2>(), npc.damage / 6, 0f, Main.myPlayer);
                        }
                    }
                }
            }

            /*if (!masoBool[0]) //is not berserk
            {
                SharkCount = 0;

                if (npc.HasPlayerTarget)
                {
                    Player player = Main.player[npc.target];
                    if (player.active && !player.dead && player.Center.Y < npc.position.Y && npc.Distance(player.Center) < 1000f)
                    {
                        Counter[1]++; //timer runs if player is above me and nearby
                        if (Counter[1] >= 600 && Main.netMode != NetmodeID.MultiplayerClient) //go berserk
                        {
                            masoBool[0] = true;
                            npc.netUpdate = true;
                            NetUpdateMaso(npc.whoAmI);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("King Slime has enraged!"), new Color(175, 75, 255));
                            else
                                Main.NewText("King Slime has enraged!", 175, 75, 255);
                        }
                    }
                    else
                    {
                        Counter[1] = 0;
                    }
                }
            }
            else //is berserk
            {
                SharkCount = 1;

                if (!masoBool[2])
                {
                    masoBool[2] = true;
                    Main.PlaySound(SoundID.Roar, npc.Center, 0);
                }

                if (Counter[0] > 45) //faster slime spike rain
                    Counter[0] = 45;

                if (++Counter[2] > 30) //aimed spikes
                {
                    Counter[2] = 0;
                    const float gravity = 0.15f;
                    float time = 45f;
                    Vector2 distance = Main.player[npc.target].Center - npc.Center + Main.player[npc.target].velocity * 30f;
                    distance.X = distance.X / time;
                    distance.Y = distance.Y / time - 0.5f * gravity * time;
                    for (int i = 0; i < 15; i++)
                    {
                        Projectile.NewProjectile(npc.Center, distance + Main.rand.NextVector2Square(-1f, 1f) * 2f,
                            ModContent.ProjectileType<SlimeSpike>(), npc.damage / 4, 0f, Main.myPlayer);
                    }
                }

                if (npc.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient && Main.player[npc.target].position.Y > npc.position.Y) //player went back down
                {
                    masoBool[0] = false;
                    masoBool[2] = false;
                    NetUpdateMaso(npc.whoAmI);
                }
            }*/

            // Drop summon
            EModeUtils.DropSummon(npc, ModContent.ItemType<SlimyCrown>(), NPC.downedSlimeKing, ref DroppedSummon);

            return true;
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            npc.DropItemInstanced(npc.position, npc.Size, ItemID.LifeCrystal, 3);
            npc.DropItemInstanced(npc.position, npc.Size, ItemID.WoodenCrate, 5);
            npc.DropItemInstanced(npc.position, npc.Size, ModContent.ItemType<SlimyShield>());

            if (Main.netMode != NetmodeID.MultiplayerClient && !FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>()) && !NPC.AnyNPCs(ModContent.NPCType<Mutant>()))
            {
                int n = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<Mutant>());
                if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Slimed, 120);
        }

        public override void LoadSprites(NPC npc, bool recolor)
        {
            base.LoadSprites(npc, recolor);

            LoadNPCSprite(recolor, npc.type);
            LoadBossHeadSprite(recolor, 7);
            LoadGore(recolor, 734);
            LoadExtra(recolor, 39);

            Main.ninjaTexture = LoadSprite(recolor, "Ninja");
        }
    }
}