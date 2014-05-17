using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TriggerHappy.Packets {
    [StructLayout(LayoutKind.Sequential, Pack=0x1)]
    public unsafe struct StrikeNPC {
        public short NPCID;
        public short Damage;
        public float Knockback;
        public byte Direction;
        public byte CrititcalHit;

        public static unsafe StrikeNPC MarshalFromBuffer(ref TerrariaApi.Server.GetDataEventArgs args) {
            StrikeNPC npc;

            fixed (byte* _buffer = args.Msg.readBuffer) {
                byte* bufferOffset = _buffer + args.Index;
                npc = *((StrikeNPC*)bufferOffset);
            }

            return npc;
        }
    }
}
