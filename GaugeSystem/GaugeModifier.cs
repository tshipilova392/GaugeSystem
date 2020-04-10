namespace Tools.Gauges
{
	public enum GaugeModType
	{
		Flat = 10,
		PercentAdd = 20,
		PercentMult = 30,
		FlatLast = 40,
	}

	public struct GaugeModifier
	{
		public readonly long Id;
		public readonly float Value;
		public readonly GaugeModType Type;

		public GaugeModifier(long id, float value, GaugeModType type)
		{
			Id = id;
			Value = value;
			Type = type;
		}
	}
}
