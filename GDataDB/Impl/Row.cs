using System;

namespace GDataDB.Impl {
    public class Row<T> : IRow<T> where T: new() {
        public T Element { get; set; }
        //public readonly DateTimeOffset Updated;
        public readonly string Etag;
        public readonly Uri Id;
        private readonly Uri Edit;
        private readonly DatabaseClient client;
        private readonly Serializer<T> serializer = new Serializer<T>();

        public Row(string etag, Uri id, Uri edit, DatabaseClient client) {
            Etag = etag;
            Id = id;
            Edit = edit;
            this.client = client;
        }

        public void Update() {
            // https://developers.google.com/google-apps/spreadsheets/#updating_a_list_row_1
            var xml = serializer.Serialize(this);

            throw new NotImplementedException();
        }

        public void Delete() {
            // https://developers.google.com/google-apps/spreadsheets/#deleting_a_list_row_1
            // DELETE https://spreadsheets.google.com/feeds/list/key/worksheetId/private/full/rowId/rowVersion
            var http = client.RequestFactory.CreateRequest();
            http.Headers.Add("If-Match", "*");
            http.UploadString(Edit, method: "DELETE", data: "");
        }
    }
}