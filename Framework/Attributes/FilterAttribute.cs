using System;

namespace TriggerHappy {
	/// <summary>
	/// FilterAttribute.  Tells the Triggerhappy engine this class is a trigger
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class FilterAttribute : Attribute {
		public string Name { get; private set; }

		public FilterAttribute(string name) {
			this.Name = name;
		}
	}
}

