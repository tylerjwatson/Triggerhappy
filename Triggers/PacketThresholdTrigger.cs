using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TriggerHappy {

    /// <summary>
    /// Packet threshold trigger.
    /// 
    /// Measures the count of packets that are received from a player per second or minute, and 
    /// pulls the trigger when the configured threshold is exceeded.
    /// </summary>
    [Trigger("PacketThreshold")]
    public class PacketThresholdTrigger : Trigger {
        System.Timers.Timer timer;
        int threshold;
        public readonly object packetsPerUserLock = new object();
        volatile Dictionary<int, long> packetsPerUser = new Dictionary<int, long>();

        public PacketThresholdTrigger(Chain parent, XElement triggerElement) : base(parent, triggerElement) {
            if (triggerElement.HasAttributes == false || triggerElement.Attribute("Threshold") == null
                || int.TryParse(triggerElement.Attribute("Threshold").Value, out threshold) == false) {
                    THLog.Log(LogLevel.Error, "Cannot set up this instance of PacketThresholdTrigger: Threshold attribute is missing or invalid.");
                    throw new ArgumentException("@Threshold");
            }
            timer = new System.Timers.Timer() {
                Interval = 1000,
                Enabled = true
            };

            timer.Elapsed += timer_Elapsed;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            lock (packetsPerUserLock) {
                foreach (var item in packetsPerUser) {
                    Console.WriteLine("{0}: {1} packets per sec", Terraria.Main.player[item.Key], item.Value);
                }
                packetsPerUser.Clear();
            }
        }

        /// <summary>
        /// Increments the packets per user counter and returns the result;
        /// </summary>
        long IncrementForUser(ref TerrariaApi.Server.GetDataEventArgs dataArgs) {
            int whoAmI = dataArgs.Msg.whoAmI;
            lock (packetsPerUserLock) {
                if (packetsPerUser.ContainsKey(whoAmI) == false) {
                    packetsPerUser.Add(whoAmI, 0);
                }

                return ++packetsPerUser[whoAmI];
            }
        }

        #region implemented abstract members of Trigger

        public override void EvalTrigger(ref TerrariaApi.Server.GetDataEventArgs dataEventArgs, ref bool stopProcessing) {
            long finalPPU;
            if ((finalPPU = IncrementForUser(ref dataEventArgs)) > threshold) {
                PullTrigger(ref dataEventArgs, "Exceeded threshold of " + threshold, ref stopProcessing);
            }
        }

        #endregion

        public override string ToString() {
            return string.Format("[PacketThresholdTrigger]");
        }
    }
}

