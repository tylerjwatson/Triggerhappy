using System;
using System.Xml.Linq;

namespace TriggerHappy {
	/// <summary>
	/// Describes a chain filter.  Chain filters help drill out whether a chain should execute or not.
	/// </summary>
	public abstract class Filter {
		public Chain parentChain;
		protected XElement filterConfigurationElement;

		public Filter(Chain parentChain, XElement filterConfigurationElement) {
			if (parentChain == null) {
				throw new ArgumentNullException("parentChain");
			}
			this.parentChain = parentChain;
			this.filterConfigurationElement = filterConfigurationElement;
		}

		/// <summary>
		/// Evals the filter.  Derived classes must implement this, and return true if the filter succeeded, 
		/// false otherwise.
		/// </summary>
		/// <returns><c>true</c>, if filter was evaled, <c>false</c> otherwise.</returns>
		/// <param name="dataEventArgs">The ${ParameterType} instance containing the event data.</param>
		public abstract bool EvalFilter(ref TerrariaApi.Server.GetDataEventArgs dataEventArgs, ref bool stopProcessing);

	}
}

