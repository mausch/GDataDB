using System;
using System.IO;
using GDataDB.Impl;
using Google.GData.Client;
using Google.GData.Documents;
using Google.GData.Spreadsheets;
using SpreadsheetQuery=Google.GData.Documents.SpreadsheetQuery;

namespace GDataDB {
	public class DatabaseClient : IDatabaseClient {
		public IService DocumentService { get; set;}
		public IService SpreadsheetService { get; set; } 

		public DatabaseClient(string username, string password) {
			var docService = new DocumentsService("database");
			docService.setUserCredentials(username, password);
			DocumentService = docService;

			var ssService = new SpreadsheetsService("database");
			ssService.setUserCredentials(username, password);
			SpreadsheetService = ssService;
		}

		public IDatabase CreateDatabase(string name) {
			using (var ms = new MemoryStream()) {
				using (var sw = new StreamWriter(ms)) {
					sw.WriteLine(",,,");
					var spreadSheet = DocumentService.Insert(new Uri(DocumentsListQuery.documentsBaseUri), ms, "text/csv", name);
					return new Database(SpreadsheetService, spreadSheet);
				}
			}
		}

		public IDatabase GetDatabase(string name) {
			var feed = DocumentService.Query(new SpreadsheetQuery {TitleExact = true, Title = name});
			if (feed.Entries.Count == 0)
				return null;
			return new Database(SpreadsheetService, feed.Entries[0]);
		}
	}
}