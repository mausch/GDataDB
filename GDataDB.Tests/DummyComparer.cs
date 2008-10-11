using System.Collections.Generic;

namespace GDataDB.Tests {
	public class DummyComparer<T>: IComparer<T> {
		public int Compare(T x, T y) {
			throw new System.NotImplementedException();
		}
	}
}