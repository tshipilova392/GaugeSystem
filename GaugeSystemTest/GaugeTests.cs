using NUnit.Framework;

namespace Tools.Gauges.Tests
{
    [TestFixture]
    public class GaugeTests
    {
        private const int FirstId = 1;
        private const int SecondId = 2;
        private const int ThirdId = 3;
        private const int FourthId = 4;
        private const int FifthId = 5;
        private const int SixthId = 6;

        [Test]
        public void TestGauge()
        {
            var gauge = new Gauge(123, 8);
            
            Assert.AreEqual(gauge.BaseValue, 8);
            Assert.AreEqual(gauge.Value, 8);
            
            gauge.AddModifier(new GaugeModifier(FirstId, 2, GaugeModType.Flat));
            
            Assert.AreEqual(gauge.BaseValue, 8);
            Assert.AreEqual(gauge.Value, 10);
            
            gauge.AddModifier(new GaugeModifier(SecondId, 0.1f, GaugeModType.PercentAdd));
            
            Assert.AreEqual(gauge.Value, 11);
            
            gauge.AddModifier(new GaugeModifier(ThirdId, 0.1f, GaugeModType.PercentAdd));
            
            Assert.AreEqual(gauge.Value, 12);

            gauge.AddModifier(new GaugeModifier(FourthId, 0.5f, GaugeModType.PercentMult));

            Assert.AreEqual(gauge.Value, 18);

            gauge.AddModifier(new GaugeModifier(FifthId, 0.5f, GaugeModType.PercentMult));

            Assert.AreEqual(gauge.Value, 27);
            
            gauge.AddModifier(new GaugeModifier(SixthId, 1f, GaugeModType.FlatLast));

            Assert.AreEqual(gauge.BaseValue, 8);
            Assert.AreEqual(gauge.Value, 28);

            gauge.RemoveModifier(FourthId);
            
            Assert.AreEqual(gauge.Value, 19);
            
            gauge.RemoveModifier(ThirdId);

            Assert.AreEqual(gauge.Value, 17.5f);
            
            gauge.RemoveModifier(SecondId);

            Assert.AreEqual(gauge.Value, 16f);
            
            gauge.RemoveModifier(FirstId);

            Assert.AreEqual(gauge.Value, 13f);
            
            gauge.RemoveModifier(FifthId);

            Assert.AreEqual(gauge.Value, 9f);
            
            gauge.RemoveModifier(SixthId);
            
            Assert.AreEqual(gauge.Value, 8f);
        }
    }
}