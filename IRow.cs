namespace GDataDB {
	public interface IRow<T> {
		T Element { get; set; }
		void Update();
		void Delete();
	}
}