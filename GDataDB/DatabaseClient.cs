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

		public IDatabase CreateDatabase(string parentRef, string name) {
            var http = RequestFactory.CreateRequest();
            var boundary = Guid.NewGuid().ToString();
            http.Headers.Set("Content-Type", string.Format("multipart/related; boundary=\"{0}\"", boundary));
            var data = new StringBuilder();
            data.AppendLine("--" + boundary);
            data.AppendLine("Content-Type: application/json; charset=UTF-8");
            data.AppendLine(JsonConvert.SerializeObject(new {
                title = name,
                fileExtension = "csv",
                parents = new[] {
                    new {
                        kind = "drive#fileLink",
                        id = parentRef,
                    },
                }
            }));
            data.AppendLine();
            data.AppendLine("--" + boundary);
            data.AppendLine("Content-Type: text/csv");
            data.AppendLine();
            data.AppendLine(",,,,,,,,,,,,,,,");
            data.AppendLine("--" + boundary + "--");
            var response = http.UploadString("https://content.googleapis.com/upload/drive/v2/files?uploadType=multipart&convert=true", data: data.ToString());
            Console.WriteLine(response);

            var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

            var docId = (string)jsonResponse["id"];
            var worksheetFeedUri = new Uri(string.Format("https://spreadsheets.google.com/feeds/worksheets/{0}/private/full", docId));
            return new Database(this, docId, worksheetFeedUri);
        }

		public IDatabase GetDatabase(string name) {
		    var uriBuilder = new UriBuilder(new Uri("https://spreadsheets.google.com/feeds/spreadsheets/private/full")) {
		        Query = "title-exact=true&title=" + name
		    };

            var http = RequestFactory.CreateRequest();
            var rawResponse = http.DownloadString(uriBuilder.Uri);
            var xmlResponse = XDocument.Parse(rawResponse);

            var feedUri = ExtractEntryContent(xmlResponse);

            //var feedUri = xmlResponse.Root
            //    .Elements(AtomNs + "")
            //    .Elements(AtomNs + "link")
            //    .Where(e => e.Attribute("rel").Value == "http://schemas.google.com/g/2005#post")
            //    .Select(e => new Uri(e.Attribute("href").Value))
            //    .FirstOrDefault();

            if (feedUri == null)
                return null;

            var id = feedUri.Segments.Reverse().Skip(2).First();
            return new Database(this, id, feedUri);
		}

        public static Uri ExtractEntryContent(XDocument xdoc) {
            return ExtractEntryContent(xdoc.Root.Elements(Utils.AtomNs + "entry"));
        }

        public static Uri ExtractEntryContent(IEnumerable<XElement> entries) {
            var selectMany = entries
                .SelectMany(e => e.Elements(Utils.AtomNs + "content"));
            var xAttributes = selectMany
                .SelectMany(e => e.Attributes("src"));
            var enumerable = xAttributes
                .Select(a => new Uri(a.Value));
            return enumerable
                .FirstOrDefault();
        }
	}
}