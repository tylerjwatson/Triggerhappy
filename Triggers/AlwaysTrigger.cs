using System;
using System.Xml.Linq;

namespace TriggerHappy {
	/// <summary>
	/// Unconditionally pulls a trigger.
	/// </summary>
	[Trigger("Always")]
	public class AlwaysTrigger : Trigger {
		public AlwaysTrigger(Chain parentChain, XElement triggerElement) : base(parentChain, triggerElement) {
		}

		#region implemented abstract members of Trigger

		public override void EvalTrigger(ref TerrariaApi.Server.GetDataEventArgs dataEventArgs, ref bool stopProcessing) {
			PullTrigger(ref dataEventArgs, "Always pulled", ref stopProcessing);
		}

		#endregion
	}
}

