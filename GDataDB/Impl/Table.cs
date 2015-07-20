using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GDataDB.Impl {
    public class Table<T> : ITable<T> where T: new() {
        private readonly DatabaseClient client;
        private readonly Uri listFeedUri;
        private readonly Uri worksheetUri;
        private readonly Serializer<T> serializer = new Serializer<T>();

        public Table(DatabaseClient client, Uri listFeedUri, Uri worksheetUri) {
            if (listFeedUri == null)
                throw new ArgumentNullException("listFeedUri");
            if (client == null)
                throw new ArgumentNullException("client");
            if (worksheetUri == null)
                throw new ArgumentNullException("worksheetUri");
            this.client = client;
            this.listFeedUri = listFeedUri;
            this.worksheetUri = worksheetUri;
        }

        public void Delete() {
            var http = client.RequestFactory.CreateRequest();
            http.UploadString(worksheetUri, method: "DELETE", data: "");
        }

        public void Clear() {
            // https://developers.google.com/google-apps/spreadsheets/#deleting_a_list_row_1
            var rows = Find(new Query {
                Order = new Order {
                    Descending = true,
                }
            });
            foreach (var row in rows)
                row.Delete();
        }

        public void Rename(string newName) {
            throw new NotImplementedException();
        }

        public IRow<T> Add(T e) {
            // https://developers.google.com/google-apps/spreadsheets/#adding_a_list_row_1
            var xml = serializer.SerializeNewRow(e);
            var http = client.RequestFactory.CreateRequest();
            var response = http.UploadString(listFeedUri, xml.ToString());
            var xmlResponse = XDocument.Parse(response);
            var row = serializer.DeserializeRow(xmlResponse.Root, client);
            return row;
        }

        public IRow<T> Get(int rowNumber) {
            throw new NotImplementedException();

            //var q = GetQuery();
            //q.StartIndex = rowNumber;
            //q.NumberToRetrieve = 1;
            //var results = Find(q);
            //if (results.Count == 0)
            //    return null;
            //return results[0];
        }

        public IList<IRow<T>> FindAll() {
            return Find(new Query());
        }

        public IList<IRow<T>> FindAll(int start, int count) {
            return Find(new Query {
                Start = start,
                Count = count,
            });
        }

        public IList<IRow<T>> Find(string query) {
            return Find(new Query {FreeQuery = query});
        }

        public IList<IRow<T>> FindStructured(string query) {
            return Find(new Query {StructuredQuery = query});
        }

        public IList<IRow<T>> FindStructured(string query, int start, int count) {
            return Find(new Query {
                StructuredQuery = query,
                Start = start,
                Count = count,
            });
        }


        public static string SerializeQuery(Query q) {
            var b = new StringBuilder();

            // TODO URLencode

            if (q.FreeQuery != null)
                b.Append("q=" + q.FreeQuery + "&");
            if (q.StructuredQuery != null)
                b.Append("sq=" + q.StructuredQuery + "&");
            if (q.Start > 0)
                b.Append("start-index=" + q.Start + "&");
            if (q.Count > 0)
                b.Append("max-results=" + q.Count + "&");
            if (q.Order != null) {
                if (q.Order.ColumnName != null)
                    b.Append("orderby=column:" + q.Order.ColumnName + "&");
                if (q.Order.Descending)
                    b.Append("reverse=true&");
            }

            return b.ToString();
        }

        public IList<IRow<T>> Find(Query q) {
            var http = client.RequestFactory.CreateRequest();
            var uriBuilder = new UriBuilder(listFeedUri) {
                Query = SerializeQuery(q),
            };
            var rawResponse = http.DownloadString(uriBuilder.Uri);
            var xmlResponse = XDocument.Parse(rawResponse);
            return xmlResponse.Root.Elements(DatabaseClient.AtomNs + "entry")
                .Select(e => serializer.DeserializeRow(e, client))
                .ToList();
        }
    }
}