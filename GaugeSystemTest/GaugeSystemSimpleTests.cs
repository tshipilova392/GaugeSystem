using System.Collections.Generic;
using NUnit.Framework;

namespace Tools.Gauges.Tests
{
    [TestFixture]
    public class GaugeSystemSimpleTests
    {
        private const int FirstId = 100;
        private const int SecondId = 200;
        
        [Test]
        public void TestSimpleGraph()
        {
            var gs = PrepareSimpleGraph();
            var holder = new GaugeHolder();
            
            gs.SetChangedAll(holder);
            
            Assert.AreEqual(holder.Stats[FirstId].BaseValue, 8);
            Assert.AreEqual(holder.Stats[FirstId].Value, 8);
            Assert.AreEqual(holder.Stats[SecondId].BaseValue, 24);
            Assert.AreEqual(holder.Stats[SecondId].Value, 24);
        }

        [Test]
        public void TestChangedBaseValue()
        {
            var gs = PrepareSimpleGraph();
            var holder = new GaugeHolder();
            
            gs.SetChangedAll(holder);

            holder.Stats[FirstId].BaseValue = 5;
            gs.SetChanged(holder, FirstId);

            Assert.AreEqual(holder.Stats[FirstId].Value, 5);
            Assert.AreEqual(holder.Stats[SecondId].Value, 15);
        }

        [Test]
        public void TestModifiers()
        {
            var gs = PrepareSimpleGraph();
            var holder = new GaugeHolder();
            
            gs.SetChangedAll(holder);

            holder.Stats[FirstId].AddModifier(new GaugeModifier(1, 3, GaugeModType.Flat));
            
            gs.SetChanged(holder, FirstId);
            
            Assert.AreEqual(holder.Stats[FirstId].Value, 11); // 8 + 3
            Assert.AreEqual(holder.Stats[SecondId].Value, 33); // 11 * 3
            
            holder.Stats[SecondId].AddModifier(new GaugeModifier(2, 5, GaugeModType.Flat));
            gs.SetChanged(holder, SecondId);
            
            Assert.AreEqual(holder.Stats[FirstId].Value, 11); // 8 + 3
            Assert.AreEqual(holder.Stats[SecondId].Value, 38); // 11 * 3 + 5

            holder.Stats[FirstId].RemoveModifier(1);
            gs.SetChanged(holder, FirstId);
            
            Assert.AreEqual(holder.Stats[FirstId].Value, 8); // 8
            Assert.AreEqual(holder.Stats[SecondId].Value, 29); // 8 * 3 + 5
        }
        
        private IGaugeSystem PrepareSimpleGraph()
        {
            var gs = new GaugeSystem();
            gs.AddIndependentGauge(FirstId, 8);
            
            //#2 = #1 * 3
            
            gs.AddDependentGauge
            (
                SecondId, 
                holder => holder.Stats[FirstId].Value * 3, 
                new List<int> {FirstId}
            );

            return gs;
        }
    }
}