using System;
using System.Xml.Linq;

namespace TriggerHappy {

    /// <summary>
    /// Packet filter.
    /// 
    /// Filters a chain to a particular MsgID from a GetData handler
    /// </summary>
    [Filter("Packet")]
    public class PacketFilter : Filter {
        int? msgId = null;
        bool anyPacket = false;

        public PacketFilter(Chain parentChain, XElement filterElement) : base(parentChain, filterElement) {
            int r;

            if (filterElement.HasAttributes == true && filterElement.Attribute("MsgID") != null) {
                string elementValue = filterElement.Attribute("MsgID").Value;
                if (elementValue.Equals("any", StringComparison.CurrentCultureIgnoreCase) == true) {
                    anyPacket = true;
                } else if (int.TryParse(elementValue, out r) == true) {
                    msgId = r;
                } else {
                    //tODO: log error
                }
            }
        }

        #region implemented abstract members of Filter

        public override bool EvalFilter(ref TerrariaApi.Server.GetDataEventArgs dataEventArgs, ref bool stopProcessing) {
            return (anyPacket == true) || dataEventArgs != null && msgId.HasValue == true && msgId.Value == (int)dataEventArgs.MsgID;
        }

        #endregion

        public override string ToString() {
            return "[PacketFilter]";
        }
    }
}

