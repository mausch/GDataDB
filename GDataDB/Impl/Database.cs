using System;
using System.Linq;
using System.Xml.Linq;

namespace GDataDB.Impl {
    public class Database : IDatabase {
        private readonly DatabaseClient client;
        private readonly string id;
        private readonly Uri worksheetFeed;

        public Database(DatabaseClient client, string id, Uri worksheetFeed) {
            this.client = client;
            this.id = id;
            this.worksheetFeed = worksheetFeed;
        }

        public ITable<T> CreateTable<T>(string name) where T: new() {
            var fields = new Serializer<T>().GetFields();

            var http = client.RequestFactory.CreateRequest();

            var request = new XDocument(
                new XElement(Utils.AtomNs + "entry",
                    new XElement(Utils.AtomNs + "title", name),
                    new XElement(Utils.SpreadsheetsNs + "rowCount", 1),
                    new XElement(Utils.SpreadsheetsNs + "colCount", fields.Count())
                    )
                );
            var response = http.UploadString(worksheetFeed.AbsoluteUri, request.ToString());
            var xmlResponse = XDocument.Parse(response);

            // i.e. https://spreadsheets.google.com/feeds/list/key/worksheetId/private/full
            var listFeedUri = DatabaseClient.ExtractEntryContent(new[] {xmlResponse.Root});

            // i.e. https://spreadsheets.google.com/feeds/worksheets/key/private/full/worksheetId/version
            var editUri = xmlResponse.Root
                .Elements(Utils.AtomNs + "link")
                .Where(e => e.Attribute("rel").Value == "edit")
                .Select(e => new Uri(e.Attribute("href").Value))
                .FirstOrDefault();

            // write headers

            var key = listFeedUri.Segments[3];
            key = key.Substring(0, key.Length - 1);
            var worksheetId = listFeedUri.Segments[4];
            worksheetId = worksheetId.Substring(0, worksheetId.Length - 1);

            var entries =
                from field in fields.Select((p, i) => new { p.Name, Index = i })
                let columnName = ((char)('A' + field.Index)).ToString() // TODO see what happens beyond Z
                let column = (field.Index + 1).ToString()
                let row = "1"
                let cell = columnName + row
                let id = string.Format("https://spreadsheets.google.com/feeds/cells/{0}/{1}/private/full/R1C{2}", key, worksheetId, column)
                let edit = id + "/1"
                select new XElement(Utils.AtomNs + "entry",
                                new XElement(Utils.BatchNs + "id", cell),
                                new XElement(Utils.BatchNs + "operation", new XAttribute("type", "update")),
                                new XElement(Utils.AtomNs + "id", id),
                                new XElement(Utils.AtomNs + "edit", 
                                    new XAttribute("rel", "edit"),
                                    new XAttribute("type", "application/atom+xml"),
                                    new XAttribute("href", edit)),
                                new XElement(Utils.SpreadsheetsNs + "cell", 
                                    new XAttribute("row", 1),
                                    new XAttribute("col", column),
                                    new XAttribute("inputValue", field.Name.ToLowerInvariant())));

            var feed = new XDocument(new XElement(Utils.AtomNs + "feed", entries));

            var batchCellUri = string.Format("https://spreadsheets.google.com/feeds/cells/{0}/{1}/private/full/batch", key, worksheetId);

            http = client.RequestFactory.CreateRequest();
            http.Headers.Add("If-Match", "*");
            http.UploadString(batchCellUri, method: "POST", data: feed.ToString());

            return new Table<T>(client, listFeedUri: listFeedUri, worksheetUri: editUri);

        }

        public ITable<T> GetTable<T>(string name) where T: new() {

            var uri = worksheetFeed + "?title-exact=true&title=" + Utils.UrlEncode(name);

            var http = client.RequestFactory.CreateRequest();
            var rawResponse = http.DownloadString(uri);
            var xmlResponse = XDocument.Parse(rawResponse);
            var feedUri = DatabaseClient.ExtractEntryContent(xmlResponse);

            if (feedUri == null)
                return null;

            var editUri = xmlResponse.Root
                .Elements(Utils.AtomNs + "entry")
                .SelectMany(e => e.Elements(Utils.AtomNs + "link"))
                .Where(e => e.Attribute("rel").Value == "edit")
                .Select(e => new Uri(e.Attribute("href").Value))
                .FirstOrDefault();

            return new Table<T>(client, listFeedUri: feedUri, worksheetUri: editUri);

            // ??? https://spreadsheets.google.com/feeds/tNXTXMh83yMWLVJfEgOWTvQ/tables
        }

        public void Delete() {
            var http = client.RequestFactory.CreateRequest();
            http.UploadString(new Uri("https://www.googleapis.com/drive/v2/files/" + id), method: "DELETE", data: "");
        }
    }
}