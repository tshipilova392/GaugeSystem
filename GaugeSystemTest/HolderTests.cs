using NUnit.Framework;

namespace Tools.Gauges.Tests
{
    [TestFixture]
    public class HolderTests
    {
        private const int FirstId = 100;
        
        [Test]
        public void TestSimpleHolder()
        {
            var holder = new GaugeHolder();
            holder.Stats[FirstId] = new Gauge(1, 3f);
            Assert.AreEqual(holder.Stats[FirstId].BaseValue, 3);
            Assert.AreEqual(holder.Stats[FirstId].Value, 3);
            
            holder.Stats[FirstId].BaseValue = 5;
            Assert.AreEqual(holder.Stats[FirstId].BaseValue, 5);
            Assert.AreEqual(holder.Stats[FirstId].Value, 5);
        }
    }
}