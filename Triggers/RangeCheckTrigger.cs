using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace TriggerHappy {
    [Trigger("RangeCheck")]
    public class RangeCheckTrigger : Trigger {
        Vector2 thresholdRange;

        static Regex thresholdStringRegex = new Regex("(\\d*?)[x,\\/\\\\\\/\\* ](\\d*)");

        public RangeCheckTrigger(Chain parentChain, XElement chainElement)
            : base(parentChain, chainElement) {
            XAttribute thAttr = null;
            Match match = null;
            int x, y;

            if (chainElement.HasAttributes == false || (thAttr = chainElement.Attribute("Range")) == null) {
                THLog.Log(LogLevel.Error, "Could not set up RangeCheckTrigger: Threshold is not specified.");
                return;
            }

            if (thresholdStringRegex.IsMatch(thAttr.Value) == false) {
                THLog.Log(LogLevel.Error, "Could not set up RangeCheckTrigger: Threshold {0} is invalid.", thAttr.Value);
                return;
            }

            if ((match = thresholdStringRegex.Match(thAttr.Value)) == null) {
                THLog.Log(LogLevel.Error, "Could not set up RangeCheckTrigger: Threshold {0} is invalid.", thAttr.Value);
                return;
            }

            if (int.TryParse(match.Groups[1].Value, out x) == true
                && int.TryParse(match.Groups[2].Value, out y) == true) {
                    thresholdRange = new Vector2(x * 16, y * 16);
            }
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
                destinationPosition = Terraria.Main.npc[strikeNpc.NPCID].position;
            }

            return VectorIntersects(playerPosition, destinationPosition, thresholdRange);
        }

        public override void EvalTrigger(ref TerrariaApi.Server.GetDataEventArgs dataEventArgs, ref bool stopProcessing) {
            if (GetRangeOfPacket(ref dataEventArgs) == false) {
                PullTrigger(ref dataEventArgs, "Packet was outside of allowable range.", ref stopProcessing);
            }
        }
    }
}
