namespace GDataDB {
	public interface IDatabaseClient {
		IDatabase CreateDatabase(string name);
		IDatabase GetDatabase(string name);
	}
}