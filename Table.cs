using System;
using System.Collections.Generic;
using System.Linq;
using Google.GData.Client;
using Google.GData.Spreadsheets;

namespace GDataDB {
	public class Table<T> : ITable<T> {
		private readonly IService svc;
		private readonly WorksheetEntry entry;
		private readonly Serializer<T> serializer = new Serializer<T>();

		public Table(IService svc, WorksheetEntry entry) {
			this.svc = svc;
			this.entry = entry;
		}

		public void Delete() {
			svc.Delete(entry); // FIXME doesn't work! throws
		}

		public void DeleteAll() {
			throw new NotImplementedException();
			//var feed = GetFeed();
			//feed.ShouldBePersisted();
			//feed.Entries.Clear();
			//feed.Publish();
		}

		private ListQuery GetQuery() {
			return new ListQuery(GetLink().HRef.Content);
		}

		private ListFeed GetFeed() {
			return (ListFeed) svc.Query(GetQuery());
		}

		private AtomLink GetLink() {
			return entry.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);
		}

		public IRow<T> Add(T e) {
			var feed = GetFeed();
			var newEntry = serializer.Serialize(e);
			var rowEntry = feed.Insert(newEntry);
			return new Row<T>((ListEntry) rowEntry) {Element = e};
		}

		public IRow<T> Get(int rowNumber) {
			var q = GetQuery();
			q.StartIndex = rowNumber;
			q.NumberToRetrieve = 1;
			var results = Find(q);
			if (results.Count == 0)
				return null;
			return results[0];
		}

		public IList<IRow<T>> FindAll() {
			return Find(GetQuery());
		}

		public IList<IRow<T>> FindAll(int start, int count) {
			var q = GetQuery();
			q.StartIndex = start;
			q.NumberToRetrieve = count;
			return Find(q);
		}

		public IList<IRow<T>> Find(string query) {
			return Find(query, null, 0, int.MaxValue);
		}

		public IList<IRow<T>> FindStructured(string query) {
			return Find(null, query, 0, int.MaxValue);
		}

		public IList<IRow<T>> Find(string query, string sq, int start, int count) {
			var q = GetQuery();
			q.Query = query;
			q.SpreadsheetQuery = sq;
			q.StartIndex = start;
			q.NumberToRetrieve = count;
			return Find(q);
		}

		private IList<IRow<T>> Find(FeedQuery q) {
			var feed = (ListFeed) svc.Query(q);
			return feed.Entries.Cast<ListEntry>()
				.Select(e => new Row<T>(e) {Element = serializer.Deserialize(e)})
				.Cast<IRow<T>>()
				.ToList();
		}
	}
}