using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TriggerHappy.UnitTests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMethod1() {
        }

        [TestMethod]
        public void TestVectorIntersects() {
            Vector2 s = new Vector2(10, 10);
            Vector2 d = new Vector2(10, 10);
            Vector2 t = new Vector2(3, 3);

            Assert.IsTrue(RangeCheckTrigger.VectorIntersects(s, d, t));

            s = new Vector2(10, 10);
            d = new Vector2(15, 15);
            t = new Vector2(3, 3);

            Assert.IsFalse(RangeCheckTrigger.VectorIntersects(s, d, t));

            s = new Vector2(13, 13);
            d = new Vector2(15, 15);
            t = new Vector2(3, 3);

            Assert.IsTrue(RangeCheckTrigger.VectorIntersects(s, d, t));
        }
    }
}
