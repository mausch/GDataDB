using System;
using System.Linq;
using GDataDB.Linq;
using NUnit.Framework;

namespace GDataDB.Tests {
	[TestFixture]
	public class QueryTranslatorTests {
		[Test]
		public void tt() {
			var qt = new QueryTranslator();
			var t = new MockTable();
			var q = new Query<Entity>(new GDataDBQueryProvider<Entity>(t));
			var iq = q.Where(e => e.Description == "pepe");
			Console.WriteLine(iq);	
		}
	}
}