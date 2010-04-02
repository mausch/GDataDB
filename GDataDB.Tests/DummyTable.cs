using System;
using System.Collections.Generic;

namespace GDataDB.Tests {
	public class DummyTable : ITable<Entity> {
		public void Delete() {
			throw new NotImplementedException();
		}

		public IRow<Entity> Add(Entity e) {
			throw new NotImplementedException();
		}

		public IRow<Entity> Get(int rowNumber) {
			throw new NotImplementedException();
		}

		public IList<IRow<Entity>> FindAll() {
			throw new NotImplementedException();
		}

		public IList<IRow<Entity>> FindAll(int start, int count) {
			throw new NotImplementedException();
		}

		public IList<IRow<Entity>> Find(string query) {
			throw new NotImplementedException();
		}

		public IList<IRow<Entity>> FindStructured(string query) {
			Console.WriteLine("FindStructured {0}", query);
			return new List<IRow<Entity>>();
		}

		public IList<IRow<Entity>> FindStructured(string query, int start, int count) {
			Console.WriteLine("FindStructured {0} start {1} count {2}", query, start, count);
			return new List<IRow<Entity>>();
		}

		public IList<IRow<Entity>> Find(Query q) {
			Console.WriteLine("Find Query");
			return new List<IRow<Entity>>();
		}

		public Uri GetFeedUrl() {
			throw new NotImplementedException();
		}
	}
}