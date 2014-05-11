using System;

namespace TriggerHappy {
	/// <summary>
	/// Action attribute.  Tells the Triggerhappy engine this class is an action
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ActionAttribute : Attribute {
		public string Name { get; private set; }

		public ActionAttribute(string name) {
			this.Name = name;
		}
	}
}

