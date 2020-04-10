using System.Collections.Generic;

namespace Tools.Gauges
{
    public interface IGaugeHolder
    {
        Dictionary<int, IGauge> Stats { get; }
    }

    public class GaugeHolder : IGaugeHolder
    {
        public Dictionary<int, IGauge> Stats { get; } = new Dictionary<int, IGauge>();
    }
}