using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TriggerHappy.Packets {
    [StructLayout(LayoutKind.Sequential, Pack=0x1)]
    unsafe struct StrikeNPCHeldItem {
        public short NPCID;
        public byte PlayerID;

        public static unsafe StrikeNPCHeldItem MarshalFromBuffer(ref TerrariaApi.Server.GetDataEventArgs args) {
            StrikeNPCHeldItem npc;

            fixed (byte* _buffer = args.Msg.readBuffer) {
                byte *bufferOffset = _buffer + args.Index;
                npc = *((StrikeNPCHeldItem*)bufferOffset);
            }

            return npc;
        }
    }
}
