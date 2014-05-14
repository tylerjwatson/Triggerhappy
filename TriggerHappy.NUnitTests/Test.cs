using NUnit.Framework;
using System;

namespace TriggerHappy.NUnitTests {
	[TestFixture]
	public class Test {
		[Test]
		public void TestChainLoad() {
			TriggerHappyPlugin plugin = new TriggerHappyPlugin(null);
			plugin.LoadTypeCache();
			//plugin.chainLoader.LoadChain("TestChain.xml");
		}
	}
}

