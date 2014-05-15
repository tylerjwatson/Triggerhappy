using System;
using System.Xml.Linq;

namespace TriggerHappy {
	/// <summary>
	/// Describes an action.
	/// 
	/// Derived classes will have an opportunity to perform an action as configured in the chain configuration.
	/// </summary>
	public abstract class Action {
		public Chain parentChain;
		protected XElement actionElement;

		public Action(Chain parentChain, XElement actionElement) {
			this.parentChain = parentChain;
			this.actionElement = actionElement;
		}

		/// <summary>
		/// Evals the action.
		/// 
		/// Derived classes must override this member to have it's action performed.
		/// </summary>
		/// <param name="dataArgs">Data arguments.</param>
		/// <param name="stopProcessing">Stop processing.</param>
		public abstract void EvalAction(ref TerrariaApi.Server.GetDataEventArgs dataArgs, ref bool stopProcessing);
	}
}

