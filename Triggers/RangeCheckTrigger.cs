using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TriggerHappy {
    [Trigger("RangeCheck")]
    public class RangeCheckTrigger : Trigger {
        Vector2 thresholdRange;

        public RangeCheckTrigger(Chain parentChain, XElement chainElement)
            : base(parentChain, chainElement) {

        }

        public static bool VectorIntersects(Vector2 source, Vector2 dest, Vector2 threshold) {
            return (dest.X >= (source.X - threshold.X) && dest.X <= (source.X + threshold.X)
                && dest.Y >= (source.Y - threshold.Y) && dest.Y <= (source.Y + threshold.Y));
        }

        bool GetRangeOfPacket(ref TerrariaApi.Server.GetDataEventArgs dataArgs) {
            Vector2 playerPosition = Terraria.Main.player[dataArgs.Msg.whoAmI].position;
            Vector2 destinationPosition = default(Vector2);

            if (dataArgs.MsgID == PacketTypes.NpcStrike) {
                Packets.StrikeNPC strikeNpc = Packets.StrikeNPC.MarshalFromBuffer(ref dataArgs);
                destinationPosition = Terraria.Main.npc[dataArgs.Msg.whoAmI].position;
            }

            return VectorIntersects(playerPosition, destinationPosition, thresholdRange);
        }

        public override void EvalTrigger(ref TerrariaApi.Server.GetDataEventArgs dataEventArgs, ref bool stopProcessing) {
            
        }

        
    }
}
