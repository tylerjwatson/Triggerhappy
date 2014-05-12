using System;
using System.Xml.Linq;

namespace TriggerHappy {

    /// <summary>
    /// Handle packet action.
    /// 
    /// This action marks a packet as "handled" in the TSAPI plugin handler, stopping the packet from 
    /// progressing any further through the plugin chain and into the Terraria server.
    /// </summary>
    [Action("HandlePacket")]
    public class HandlePacketAction : Action {
        public HandlePacketAction(Chain parentChain, XElement actionElement) : base(parentChain, actionElement) {
        }

        #region implemented abstract members of Action

        public override void EvalAction(ref TerrariaApi.Server.GetDataEventArgs dataArgs, ref bool stopProcessing) {
            dataArgs.Handled = true;
        }

        #endregion
    }
}

