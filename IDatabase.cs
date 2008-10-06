namespace GDataDB {
	public interface IDatabase {
		ITable<T> CreateTable<T>(string name);
		ITable<T> GetTable<T>(string name);
		void Delete();
	}
}