using System;
using System.Linq;
using GDataDB.Linq;
using NUnit.Framework;

namespace GDataDB.Tests {
	[TestFixture]
	public class QueryTranslatorTests {
		[Test]
		public void QueryTranslator() {
			var qt = new QueryTranslator();
			var t = new MockTable();
			var q = new Query<Entity>(new GDataDBQueryProvider<Entity>(t));
			var iq = q.Where(e => e.Description == "pepe");
			Assert.AreEqual("(description=pepe)", iq.ToString());
		}

		[Test]
		public void Linq() {
			var t = new MockTable();
			var q = from r in t.AsQueryable()
			        where r.Description == "pepe"
			        select r;

			foreach (var e in q) {
				Console.WriteLine(e.Description);
			}
		}
	}
}