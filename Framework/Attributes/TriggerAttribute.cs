using System;

namespace TriggerHappy {
	/// <summary>
	/// Trigger attribute.  Tells the triggerhappy engine that this class is a trigger.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class TriggerAttribute : Attribute {
		public string Name { get; private set; }

		public TriggerAttribute(string TriggerName) {
			this.Name = TriggerName;
		}
	}
}

