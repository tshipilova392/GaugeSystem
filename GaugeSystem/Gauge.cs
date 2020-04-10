using System;
using System.Collections.Generic;

namespace Tools.Gauges
{
	public interface IGauge
	{
		int Id { get; }
		float BaseValue { get; set; }
		float Value { get; }

		void AddModifier(GaugeModifier mod);
		bool RemoveModifier(long modifierId);
	}
	
	public class Gauge : IGauge
	{
		public int Id { get; }

		private float baseValue;
		public float BaseValue
		{
			get => baseValue;
			set
			{
				baseValue = value;
				isDirty = true;
			}
		}

		private bool isDirty = true;

		private float cachedValue = 0f;
		public float Value 
		{
			get 
			{
				if (isDirty) 
				{
					cachedValue = CalculateFinalValue();
					isDirty = false;
				}
				return cachedValue;
			}
		}

		private readonly List<GaugeModifier> statModifiers = new List<GaugeModifier>();
		public IReadOnlyList<GaugeModifier> StatModifiers => statModifiers;

		public Gauge(int id, float baseValue = 0f)
		{
			Id = id;
			this.baseValue = baseValue;
		}

		public void AddModifier(GaugeModifier mod)
		{
			isDirty = true;
			statModifiers.Add(mod);
		}

		public bool RemoveModifier(long modifierId)
		{
			var index = statModifiers.FindIndex(mod => mod.Id == modifierId);
			if (index == -1)
				return false;

			isDirty = true;
			statModifiers.RemoveAt(index);
			return true;
		}

		public bool RemoveModifier(GaugeModifier mod)
		{
			return RemoveModifier(mod.Id);
		}
		
		private float CalculateFinalValue()
		{	
			var sumFlat = 0f;
			var sumPercentAdd = 0f;
			double sumPercentMult = 1f;
			var sumFlatLast = 0f;

			foreach (var statModifier in statModifiers)
			{
				switch (statModifier.Type)
				{
					case GaugeModType.Flat:
						sumFlat += statModifier.Value;
						break;
					case GaugeModType.PercentAdd:
						sumPercentAdd += statModifier.Value;
						break;
					case GaugeModType.PercentMult:
						sumPercentMult *= (1 + statModifier.Value);
						break;
					case GaugeModType.FlatLast:
						sumFlatLast += statModifier.Value;
						break;
				}
			}
			
			var result = (baseValue + sumFlat) * (1 + sumPercentAdd) * sumPercentMult + sumFlatLast;
			return (float) Math.Round(result, 4);
		}
    }
}
