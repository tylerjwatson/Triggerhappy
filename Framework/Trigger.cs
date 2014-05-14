using System;
using System.Xml.Linq;

namespace TriggerHappy {
	/// <summary>
	/// Trigger base that all enlisted triggers must inherit from.
	/// </summary>
	public abstract class Trigger {
		public Chain parentChain;
		protected XElement triggerElement;

		public Trigger(Chain parent, XElement triggerElement) {
			this.parentChain = parent;
			this.triggerElement = triggerElement;
		}

		/// <summary>
		/// Pulls the trigger.
		/// 
		/// Pulling the trigger means that the trigger has exceeded its sanity check and the chain is to perform its actions.
		/// </summary>
		/// <param name="dataEventArgs">The ${ParameterType} instance containing the event data.</param>
		/// <param name="message">Message.</param>
		protected virtual void PullTrigger(ref TerrariaApi.Server.GetDataEventArgs dataEventArgs, string message, ref bool stopProcessing, bool silentPull = false) {
			parentChain.TriggerPulled(this, ref dataEventArgs, silentPull);

			stopProcessing = true;
		}

		/// <summary>
		/// Evals the trigger.
		/// 
		/// Forces the trigger to evaluate the data.  If the trigger is to pull, it must call PullTrigger().
		/// </summary>
		/// <param name="dataEventArgs">The ${ParameterType} instance containing the event data.</param>
		/// <param name="stopProcessing">Stop processing.</param>
		public abstract void EvalTrigger(ref TerrariaApi.Server.GetDataEventArgs dataEventArgs, ref bool stopProcessing);
	}
}

