using System.Collections.Generic;
using NUnit.Framework;

namespace Tools.Gauges.Tests
{
    [TestFixture]
    public class GaugeSystemDependencyTests
    {
        private const int FirstId = 100;
        private const int SecondId = 200;
        private const int ThirdId = 300;
        private const int FourthId = 400;
        private const int FifthId = 500;
        private const int SixthId = 600;
        private const int SeventhId = 700;

        public const int EightsId = 8;
        public const int NinesId = 9;
        public const int TensId = 10;

        [Test]
        public void CalculationTest()
        {
            var gs = PrepareSecondGraph();
            var holder = new GaugeHolder();

            gs.SetChangedAll(holder);
            
            Assert.AreEqual(holder.Stats[FirstId].Value, 1);
            Assert.AreEqual(holder.Stats[SecondId].Value, 2);
            Assert.AreEqual(holder.Stats[ThirdId].Value, 3);
            Assert.AreEqual(holder.Stats[FourthId].Value, 3);
            Assert.AreEqual(holder.Stats[FifthId].Value, 5);
            Assert.AreEqual(holder.Stats[SixthId].Value, 4);
            Assert.AreEqual(holder.Stats[SeventhId].Value, 9);

            holder.Stats[FirstId].BaseValue = 10;
            holder.Stats[SecondId].BaseValue = 20;
            holder.Stats[ThirdId].BaseValue = 30;
            
            gs.SetChanged(holder, FirstId);
            gs.SetChanged(holder, SecondId);
            gs.SetChanged(holder, ThirdId);
            
            Assert.AreEqual(holder.Stats[FirstId].Value, 10);
            Assert.AreEqual(holder.Stats[SecondId].Value, 20);
            Assert.AreEqual(holder.Stats[ThirdId].Value, 30);
            Assert.AreEqual(holder.Stats[FourthId].Value, 30);
            Assert.AreEqual(holder.Stats[FifthId].Value, 50);
            Assert.AreEqual(holder.Stats[SixthId].Value, 301);
            Assert.AreEqual(holder.Stats[SeventhId].Value, 351);
        }
        
        [Test]
        public void ModificationTest()
        {
            var gs = PrepareSecondGraph();
            var holder = new GaugeHolder();

            gs.SetChangedAll(holder);

            holder.Stats[FirstId].AddModifier(new GaugeModifier(EightsId, 5f, GaugeModType.Flat));
            gs.SetChanged(holder, FirstId);

            Assert.AreEqual(holder.Stats[FirstId].Value, 6); // 1 + 5
            Assert.AreEqual(holder.Stats[SecondId].Value, 2);
            Assert.AreEqual(holder.Stats[ThirdId].Value, 3);
            Assert.AreEqual(holder.Stats[FourthId].Value, 18);
            Assert.AreEqual(holder.Stats[FifthId].Value, 20);
            Assert.AreEqual(holder.Stats[SixthId].Value, 19);
            Assert.AreEqual(holder.Stats[SeventhId].Value, 39);

            holder.Stats[FourthId].AddModifier(new GaugeModifier(NinesId, 5f, GaugeModType.Flat));
            gs.SetChanged(holder, FourthId);

            Assert.AreEqual(holder.Stats[FirstId].Value, 6); // 1 + 5
            Assert.AreEqual(holder.Stats[SecondId].Value, 2);
            Assert.AreEqual(holder.Stats[ThirdId].Value, 3);
            Assert.AreEqual(holder.Stats[FourthId].Value, 23); // 6 * 3 + 5
            Assert.AreEqual(holder.Stats[FifthId].Value, 25);
            Assert.AreEqual(holder.Stats[SixthId].Value, 19);
            Assert.AreEqual(holder.Stats[SeventhId].Value, 44);
            
            holder.Stats[SeventhId].AddModifier(new GaugeModifier(TensId, 6f, GaugeModType.Flat));
            gs.SetChanged(holder, SeventhId);
            
            Assert.AreEqual(holder.Stats[FirstId].Value, 6); // 1 + 5
            Assert.AreEqual(holder.Stats[SecondId].Value, 2);
            Assert.AreEqual(holder.Stats[ThirdId].Value, 3);
            Assert.AreEqual(holder.Stats[FourthId].Value, 23); // 6 * 3 + 5
            Assert.AreEqual(holder.Stats[FifthId].Value, 25);
            Assert.AreEqual(holder.Stats[SixthId].Value, 19);
            Assert.AreEqual(holder.Stats[SeventhId].Value, 50); // 25 + 19 + 6

            holder.Stats[FourthId].RemoveModifier(NinesId);
            gs.SetChanged(holder, FourthId);
            
            Assert.AreEqual(holder.Stats[FirstId].Value, 6); // 1 + 5
            Assert.AreEqual(holder.Stats[SecondId].Value, 2);
            Assert.AreEqual(holder.Stats[ThirdId].Value, 3);
            Assert.AreEqual(holder.Stats[FourthId].Value, 18); // 6 * 3
            Assert.AreEqual(holder.Stats[FifthId].Value, 20);
            Assert.AreEqual(holder.Stats[SixthId].Value, 19);
            Assert.AreEqual(holder.Stats[SeventhId].Value, 45); // 20 + 19 + 6
            
            holder.Stats[FirstId].RemoveModifier(EightsId);
            gs.SetChanged(holder, FirstId);
            
            Assert.AreEqual(holder.Stats[FirstId].Value, 1); // 1
            Assert.AreEqual(holder.Stats[SecondId].Value, 2);
            Assert.AreEqual(holder.Stats[ThirdId].Value, 3);
            Assert.AreEqual(holder.Stats[FourthId].Value, 3); // 1 * 3
            Assert.AreEqual(holder.Stats[FifthId].Value, 5);
            Assert.AreEqual(holder.Stats[SixthId].Value, 4);
            Assert.AreEqual(holder.Stats[SeventhId].Value, 15); // 5 + 4 + 6
            
            holder.Stats[SeventhId].RemoveModifier(TensId);
            gs.SetChanged(holder, SeventhId);
            
            Assert.AreEqual(holder.Stats[FirstId].Value, 1); // 1
            Assert.AreEqual(holder.Stats[SecondId].Value, 2);
            Assert.AreEqual(holder.Stats[ThirdId].Value, 3);
            Assert.AreEqual(holder.Stats[FourthId].Value, 3); // 1 * 3
            Assert.AreEqual(holder.Stats[FifthId].Value, 5);
            Assert.AreEqual(holder.Stats[SixthId].Value, 4);
            Assert.AreEqual(holder.Stats[SeventhId].Value, 9); // 5 + 4 
        }
        
        /// <summary>
        /// #1 = 1
        /// #2 = 2
        /// #3 = 3
        /// #4 = #1 * 3;
        /// #5 = #2 + #4;
        /// #6 = #1 * #3 + 1;
        /// #7 = #5 + #6;
        /// </summary>
        /// <returns></returns>
        private IGaugeSystem PrepareSecondGraph()
        {
            var gs = new GaugeSystem();
            
            gs.AddIndependentGauge(FirstId, 1);
            gs.AddIndependentGauge(SecondId, 2);
            gs.AddIndependentGauge(ThirdId, 3);
            
            gs.AddDependentGauge
            (
                FourthId, 
                holder => holder.Stats[FirstId].Value * 3, 
                new List<int> {FirstId}
            );

            gs.AddDependentGauge
            (
                FifthId, 
                holder => holder.Stats[SecondId].Value + holder.Stats[FourthId].Value, 
                new List<int> {SecondId, FourthId}
            );

            gs.AddDependentGauge
            (
                SixthId, 
                holder => holder.Stats[ThirdId].Value * holder.Stats[FirstId].Value + 1, 
                new List<int> {ThirdId, FirstId}
            );

            gs.AddDependentGauge
            (
                SeventhId, 
                holder => holder.Stats[FifthId].Value + holder.Stats[SixthId].Value, 
                new List<int> {FifthId, SixthId}
            );

            return gs;
        }
    }
}