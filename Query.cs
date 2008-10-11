namespace GDataDB {
	public class Query {
		public int Start { get; set; }
		public int Count { get; set; }
		public string FreeQuery { get; set; }
		public string StructuredQuery { get; set; }
		private Order Order { get; set; }
	}
}