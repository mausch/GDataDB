using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using GDataDB.Impl;
using Newtonsoft.Json;

namespace GDataDB {
	public class DatabaseClient : IDatabaseClient {

        public readonly GDataDBRequestFactory RequestFactory;

		public DatabaseClient(string clientEmail, byte[] privateKey) {
            if (clientEmail == null)
                throw new ArgumentNullException("clientEmail");
            if (privateKey == null)
                throw new ArgumentNullException("privateKey");

            RequestFactory = new GDataDBRequestFactory(clientEmail, privateKey);
		}

        private static string CreateDatabaseContent(string boundary, string name) {
            var data = new StringBuilder();
            data.AppendLine("--" + boundary);
            data.AppendLine("Content-Type: application/json; charset=UTF-8");
            data.AppendLine(JsonConvert.SerializeObject(new {
                title = name,
                fileExtension = "csv",
                parents = new[] {
                    new {
                        kind = "drive#fileLink",
                        id = "",
                    },
                }
            }));
            data.AppendLine();
            data.AppendLine("--" + boundary);
            data.AppendLine("Content-Type: text/csv");
            data.AppendLine();
            data.AppendLine(",,,,,,,,,,,,,,,");
            data.AppendLine("--" + boundary + "--");
            return data.ToString();
        }

        public IDatabase CreateDatabase(string name) {
            var http = RequestFactory.CreateRequest();
            var boundary = Guid.NewGuid().ToString();
            http.Headers.Set("Content-Type", string.Format("multipart/related; boundary=\"{0}\"", boundary));
            var data = CreateDatabaseContent(boundary: boundary, name: name);
            var response = http.UploadString("https://content.googleapis.com/upload/drive/v2/files?uploadType=multipart&convert=true", data: data);

            var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

            var docId = (string)jsonResponse["id"];
            var worksheetFeedUri = new Uri(string.Format("https://spreadsheets.google.com/feeds/worksheets/{0}/private/full", docId));
            return new Database(this, docId, worksheetFeedUri);
        }

		public IDatabase GetDatabase(string name) {

            var uri = "https://spreadsheets.google.com/feeds/spreadsheets/private/full?title-exact=true&title=" + Utils.UrlEncode(name);

            var http = RequestFactory.CreateRequest();
            var rawResponse = http.DownloadString(uri);
            var xmlResponse = XDocument.Parse(rawResponse);

            var feedUri = ExtractEntryContent(xmlResponse);

            if (feedUri == null)
                return null;

            var id = feedUri.Segments.Reverse().Skip(2).First();
            return new Database(this, id, feedUri);
		}

        public static Uri ExtractEntryContent(XDocument xdoc) {
            return ExtractEntryContent(xdoc.Root.Elements(Utils.AtomNs + "entry"));
        }

        public static Uri ExtractEntryContent(IEnumerable<XElement> entries) {
            return entries
                .SelectMany(e => e.Elements(Utils.AtomNs + "content"))
                .SelectMany(e => e.Attributes("src"))
                .Select(a => new Uri(a.Value))
                .FirstOrDefault();
        }
	}
}