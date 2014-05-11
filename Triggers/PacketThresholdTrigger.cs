using System;
using System.Xml.Linq;

namespace TriggerHappy {

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
	}
}

