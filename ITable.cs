using System.Collections.Generic;

namespace GDataDB {
	public interface ITable<T> {
		void Delete();
		void DeleteAll();
		IRow<T> Add(T e);
		IRow<T> Get(int rowNumber);
		IList<IRow<T>> FindAll();
		IList<IRow<T>> FindAll(int start, int count);
		IList<IRow<T>> Find(string query);
		IList<IRow<T>> FindStructured(string query);
		IList<IRow<T>> FindStructured(string query, int start, int count);
		IList<IRow<T>> Find(Query q);
	}
}