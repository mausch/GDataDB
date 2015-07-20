using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace GDataDB.Impl {
    /// <summary>
    /// (de)serializes an object into a spreadsheet row
    /// Uses only the object properties.
    /// Property names are used as column names in the spreadsheet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Serializer<T> where T: new() {
        public static readonly XNamespace GsxNs = "http://schemas.google.com/spreadsheets/2006/extended";
        public static readonly XNamespace GdNs = "http://schemas.google.com/g/2005";

        public XElement SerializeNewRow(T e) {
            return new XElement(DatabaseClient.AtomNs + "entry",
                new XAttribute(XNamespace.Xmlns + "gsx", GsxNs),
                //new XElement(DatabaseClient.AtomNs + "id", "123"),
                SerializeFields(e).ToArray());
        }

        public IEnumerable<XElement> SerializeFields(T e) {
            return 
                from p in typeof(T).GetProperties() 
                where p.CanRead 
                select new XElement(GsxNs + p.Name.ToLowerInvariant(), ToNullOrString(p.GetValue(e, null)));
        }

        public XElement Serialize(Row<T> row) {
            var e = new XElement(DatabaseClient.AtomNs + "entry", 
                new XAttribute(GdNs + "etag", row.Etag));
            e.Add(new XElement(DatabaseClient.AtomNs + "id", row.Id.AbsoluteUri));
            e.Add(SerializeFields(row.Element));
            return e;
        }

        public string ToNullOrString(object o) {
            if (o == null)
                return null;
            return o.ToString();
        }

        public object ConvertFrom(object value, Type t) {
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof (Nullable<>))) {
                var nc = new NullableConverter(t);
                return nc.ConvertFrom(value);
            }
            return Convert.ChangeType(value, t);
        }

        public T DeserializeElement(XElement entry) {
            var setters = 
                entry.Elements()
                    .Where(e => e.Name.Namespace == GsxNs)
                    .Select(e => new {
                        property = new[] { typeof(T).GetProperty(e.Name.LocalName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public) }
                                    .FirstOrDefault(x => x != null && x.CanWrite),
                        rawValue = e.Value,
                    })
                    .Where(e => e.property != null)
                    .Select(e => new {
                        e.property,
                        value = ConvertFrom(e.rawValue, e.property.PropertyType),
                    });
            var r = new T();
            foreach (var setter in setters)
                setter.property.SetValue(r, setter.value, null);
            return r;
        }

        public IRow<T> DeserializeRow(XElement entry, DatabaseClient client) {
            var etag = entry.Attribute(GdNs + "etag").Value;
            var id = new Uri(entry.Element(DatabaseClient.AtomNs + "id").Value);
            var edit = entry
                .Elements(DatabaseClient.AtomNs + "link")
                .Where(e => e.Attribute("rel").Value == "edit")
                .Select(e => new Uri(e.Attribute("href").Value))
                .FirstOrDefault();

            var value = DeserializeElement(entry);
            return new Row<T>(etag, id: id, edit: edit, client: client) {
                Element = value,
            };
        }
    }
}