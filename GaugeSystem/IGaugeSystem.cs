using System;
using System.Collections.Generic;
using GaugeSystem;

namespace Tools.Gauges
{
    public interface IGaugeSystem
    {
        void AddIndependentGauge(int gaugeId, float baseValue);
        void AddDependentGauge(int gaugeId, Func<IGaugeHolder, float> calculateFunc, List<int> dependencies);
        void ClearAll();

        void SetChanged(IGaugeHolder holder, int gaugeId);
        void SetChangedAll(IGaugeHolder holder);
		
//        void SetValue(IGaugeHolder holder, int modifierId);
//        void AddModifier(IGaugeHolder holder, int gaugeId, GaugeModifier modifier);
//        bool RemoveModifier(IGaugeHolder holder, int gaugeId, long modifierId);
    }

    public class GaugeSystem : IGaugeSystem
    {
        Graph<Gauge> graph = new Graph<Gauge>();
        GaugeHolder gaugeHolder = new GaugeHolder();
        Dictionary<int, Func<IGaugeHolder, float>> funcHolder = new Dictionary<int, Func<IGaugeHolder, float>>();
        public void AddIndependentGauge(int gaugeId, float baseValue)
        {
            Gauge newGauge = new Gauge(gaugeId, baseValue);
            graph.AddNode(newGauge);
            gaugeHolder.Stats.Add(gaugeId, newGauge);
        }

        public void AddDependentGauge(int gaugeId, Func<IGaugeHolder, float> calculateFunc, List<int> dependencies)
        {
            Gauge fromGauge = new Gauge(gaugeId, calculateFunc(gaugeHolder));
            funcHolder.Add(gaugeId, calculateFunc);
            foreach (var e in dependencies)
            {
                graph.AddEdge(fromGauge, (Gauge)gaugeHolder.Stats[e]);
            }
            gaugeHolder.Stats.Add(gaugeId, fromGauge);
        }

        public void ClearAll()
        {
            throw new NotImplementedException();
        }

        public void SetChanged(IGaugeHolder holder, int gaugeId)
        {
            Gauge tmpGauge = (Gauge)holder.Stats[gaugeId];
            gaugeHolder.Stats[gaugeId].BaseValue = tmpGauge.BaseValue;
            foreach (var gauge in graph.BreadthSearch(tmpGauge))
            {
                if (funcHolder.ContainsKey(gauge.Id))
                {
                    var calculatedValue = funcHolder[gauge.Id](gaugeHolder);
                    gauge.BaseValue = calculatedValue;
                    holder.Stats[gauge.Id].BaseValue = calculatedValue;
                }
            }
        }

        public void SetChangedAll(IGaugeHolder holder)
        {
            foreach (var e in gaugeHolder.Stats)
                holder.Stats.Add(e.Key, e.Value);
        }
    }
}