using System;
using System.Xml.Linq;

namespace TriggerHappy {

    /// <summary>
    /// Packet threshold trigger.
    /// 
    /// Measures the count of packets that are received from a player per second, or minute, and 
    /// pulls the trigger when the configured threshold is exceeded.
    /// </summary>
    [Trigger("PacketThreshold")]
    public class PacketThresholdTrigger : Trigger {
        System.Timers.Timer timer;

        public PacketThresholdTrigger(Chain parent, XElement triggerElement) : base(parent, triggerElement) {
            timer = null;
        }

        #region implemented abstract members of Trigger

        public override void EvalTrigger(ref TerrariaApi.Server.GetDataEventArgs dataEventArgs, ref bool stopProcessing) {

        }

        #endregion

        public override string ToString() {
            return string.Format("[PacketThresholdTrigger]");
        }
    }
}

