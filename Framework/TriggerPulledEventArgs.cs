using System;

namespace TriggerHappy {
	public class TriggerPulledEventArgs : EventArgs {
		public Trigger Trigger { get; protected set; }
		public string Message { get; protected set; }
		public TerrariaApi.Server.GetDataEventArgs DataArgs { get; protected set; }

		public TriggerPulledEventArgs(Trigger trigger, string message, ref TerrariaApi.Server.GetDataEventArgs dataArgs) {
			this.Trigger = trigger;
			this.Message = message;
			this.DataArgs = dataArgs;
		}
	}
}

