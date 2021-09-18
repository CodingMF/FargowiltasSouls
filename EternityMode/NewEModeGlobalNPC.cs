﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode
{
    public class NewEModeGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public List<EModeNPCBehaviour> EModeNpcBehaviours = new List<EModeNPCBehaviour>();

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            if (!FargoSoulsWorld.MasochistMode)
                return;

            InitBehaviourList(npc);

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.SetDefaults(npc);
            }

            bool recolor = SoulConfig.Instance.BossRecolors && FargoSoulsWorld.MasochistMode;
            if (recolor || Fargowiltas.Instance.LoadedNewSprites)
            {
                Fargowiltas.Instance.LoadedNewSprites = true;
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.LoadSprites(npc, recolor);
                }
            }
        }

        private void InitBehaviourList(NPC npc)
        {
            // TODO Try caching this again? Last attempt caused major fails
            IEnumerable<EModeNPCBehaviour> behaviours = EModeNPCBehaviour.AllEModeNpcBehaviours
                .Where(m => m.Matcher.Satisfies(npc.type));

            // To make sure they're always in the same order
            // TODO is ordering needed? Do they always have the same order?
            behaviours.OrderBy(m => m.GetType().FullName, StringComparer.InvariantCulture);

            EModeNpcBehaviours = behaviours.Select(m => m.NewInstance()).ToList();
        }

        #region Behaviour Hooks
        public override bool PreAI(NPC npc)
        {
            if (!FargoSoulsWorld.MasochistMode)
                return true;

            bool result = true;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                result &= behaviour.PreAI(npc);
            }

            return result;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (!FargoSoulsWorld.MasochistMode)
                return;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.AI(npc);
            }
        }

        public override void NPCLoot(NPC npc)
        {
            base.NPCLoot(npc);

            if (!FargoSoulsWorld.MasochistMode)
                return;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.NPCLoot(npc);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            if (!FargoSoulsWorld.MasochistMode)
                return;

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.OnHitPlayer(npc, target, damage, crit);
            }
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            bool result = base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);

            if (FargoSoulsWorld.MasochistMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    result &= behaviour.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
                }
            }

            return result;
        }

        public override bool CheckDead(NPC npc)
        {
            bool result = base.CheckDead(npc);

            if (FargoSoulsWorld.MasochistMode)
            {
                foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
                {
                    behaviour.CheckDead(npc);
                }
            }

            return result;
        }

        public void NetSync(byte whoAmI)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = mod.GetPacket();
            packet.Write((byte)22); // New maso sync packet id
            packet.Write(whoAmI);

            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.NetSend(packet);
            }

            packet.Send();
        }

        public void NetRecieve(BinaryReader reader)
        {
            foreach (EModeNPCBehaviour behaviour in EModeNpcBehaviours)
            {
                behaviour.NetRecieve(reader);
            }
        }
        #endregion
    }
}