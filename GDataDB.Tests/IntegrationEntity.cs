using System;

namespace GDataDB.Tests {
	public class IntegrationEntity {
		public int IntProp { get; set; }
		public int? IntNullProp { get; set; }
		public string StringProp { get; set; }
		public DateTime DateTimeProp { get; set; }
		public DateTime? DateTimeNullProp { get; set; }
		public float FloatProp { get; set; }
		public float? FloatNullProp { get; set; }
		public double DoubleProp { get; set; }
		public double? DoubleNullProp { get; set; }
		public decimal DecimalProp { get; set; }
		public decimal? DecimalNullProp { get; set; }
	}
}