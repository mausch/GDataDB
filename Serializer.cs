using System;
using System.Reflection;
using Google.GData.Spreadsheets;

namespace GDataDB {
	public class Serializer<T> {
		public ListEntry Serialize(T e) {
			return Serialize(e, new ListEntry());
		}

		public ListEntry Serialize(T e, ListEntry record) {
			foreach (var p in typeof (T).GetProperties()) {
				record.Elements.Add(new ListEntry.Custom {
					LocalName = p.Name.ToLowerInvariant(), // for some reason this HAS to be lowercase or it throws
					Value = ToNullOrString(p.GetValue(e, null)),
				});
			}
			return record;
		}

		public string ToNullOrString(object o) {
			if (o == null)
				return null;
			return o.ToString();
		}

		public T Deserialize(ListEntry e) {
			var r = (T)Activator.CreateInstance(typeof(T));
			foreach (ListEntry.Custom c in e.Elements) {
				var property = typeof(T).GetProperty(c.LocalName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
				var value = Convert.ChangeType(c.Value, property.PropertyType);
				property.SetValue(r, value, null);
			}
			return r;
		}

	}
}