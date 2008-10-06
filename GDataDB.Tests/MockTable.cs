using System;
using System.Collections.Generic;

namespace GDataDB.Tests {
	public class Entity {
		public string Description { get; set; }
	}

	public class MockTable : ITable<Entity> {
		public void Delete() {
			throw new NotImplementedException();
		}

		public void DeleteAll() {
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
			throw new NotImplementedException();
		}

		public IList<IRow<Entity>> Find(string query, string sq, int start, int count) {
			throw new NotImplementedException();
		}
	}
}